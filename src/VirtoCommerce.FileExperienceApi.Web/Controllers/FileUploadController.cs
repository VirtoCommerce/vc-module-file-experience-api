using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.AssetsModule.Core.Swagger;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.FileExperienceApi.Web.Filters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Helpers;

namespace VirtoCommerce.FileExperienceApi.Web.Controllers;

[Route("api/files")]
[Authorize]
public class FileUploadController : Controller
{
    private readonly IFileUploadService _fileUploadService;
    private static readonly FormOptions _defaultFormOptions = new();

    public FileUploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpGet("{scope}/options")]
    public ActionResult<FileUploadOptions> GetOptions([FromRoute] string scope)
    {
        ActionResult result = TryGetOptions(scope, out var options)
            ? Ok(options)
            : NotFound();

        return result;
    }

    [HttpPost("{scope}")]
    [UploadFile]
    [DisableFormValueModelBinding]
    public async Task<ActionResult<IList<FileUploadResult>>> UploadFiles([FromRoute] string scope)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-6.0
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            return new[] { FileUploadError.InvalidContentType(Request.ContentType) };
        }

        var results = new List<FileUploadResult>();

        try
        {
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                    MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                {
                    var fileName = contentDisposition.FileName.Value;

                    try
                    {
                        var request = AbstractTypeFactory<FileUploadRequest>.TryCreateInstance();
                        request.Scope = scope;
                        request.UserId = GetUserId();
                        request.FileName = fileName;
                        request.Stream = section.Body;

                        var result = await _fileUploadService.UploadFileAsync(request);

                        if (result.Url != null)
                        {
                            // Hide real URL
                            result.Url = Url.Action(nameof(DownloadFile), new { result.Id });
                        }

                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        results.Add(FileUploadError.Exception(ex, fileName));
                    }
                }
                else
                {
                    results.Add(FileUploadError.InvalidContent());
                }

                section = await reader.ReadNextSectionAsync();
            }
        }
        catch (Exception ex)
        {
            results.Add(FileUploadError.Exception(ex));
        }

        return results;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFile([FromRoute] string id)
    {
        await _fileUploadService.DeleteFilesAsync(new[] { id });
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetFile([FromRoute] string id)
    {
        var file = await _fileUploadService.OpenReadAsync(id);

        if (file?.Stream is null)
        {
            return NotFound();
        }

        return File(file.Stream, file.ContentType, file.Name);
    }


    private bool TryGetOptions(string scope, out FileUploadScopeOptions options)
    {
        options = _fileUploadService.GetOptions(scope);
        return options != null;
    }

    private string GetUserId()
    {
        return
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("name") ??
            AnonymousUser.UserName;
    }
}
