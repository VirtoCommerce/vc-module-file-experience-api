using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.AssetsModule.Core.Swagger;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.FileExperienceApi.Web.Filters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Helpers;
using FilePermissions = VirtoCommerce.FileExperienceApi.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.FileExperienceApi.Web.Controllers;

[Route("api/files")]
[Authorize]
public class FileUploadController : Controller
{
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileAuthorizationService _fileAuthorizationService;
    private static readonly FormOptions _defaultFormOptions = new();

    public FileUploadController(
        IFileUploadService fileUploadService,
        IFileAuthorizationService fileAuthorizationService)
    {
        _fileUploadService = fileUploadService;
        _fileAuthorizationService = fileAuthorizationService;
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
                    var result = await SaveFile(contentDisposition.FileName.Value, section.Body);
                    results.Add(result);
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

        async Task<FileUploadResult> SaveFile(string fileName, Stream stream)
        {
            try
            {
                var request = AbstractTypeFactory<FileUploadRequest>.TryCreateInstance();
                request.Scope = scope;
                request.UserId = GetUserId();
                request.FileName = fileName;
                request.Stream = stream;

                var result = await _fileUploadService.UploadFileAsync(request);

                if (result.Url != null)
                {
                    // Hide real URL
                    result.Url = Url.Action(nameof(DownloadFile), new { result.Id });
                }

                return result;
            }
            catch (Exception ex)
            {
                return FileUploadError.Exception(ex, fileName);
            }
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> DownloadFile([FromRoute] string id)
    {
        var file = await _fileUploadService.GetNoCloneAsync(id);
        if (file is null)
        {
            return NotFound();
        }

        var authorizationResult = await _fileAuthorizationService.AuthorizeAsync(User, file, FilePermissions.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var stream = await _fileUploadService.OpenReadAsync(id);
        return File(stream, file.ContentType, file.Name);
    }


    private string GetUserId()
    {
        return
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("name") ??
            AnonymousUser.UserName;
    }
}
