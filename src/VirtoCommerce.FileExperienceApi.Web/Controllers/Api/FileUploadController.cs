using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.FileExperienceApi.Web.Filters;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Swagger;
using VirtoCommerce.Platform.Data.Helpers;
using static VirtoCommerce.Xapi.Core.ModuleConstants;
using FilePermissions = VirtoCommerce.FileExperienceApi.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.FileExperienceApi.Web.Controllers.Api;

[Authorize]
[AllowAnonymous]
[Route("api/files")]
public class FileUploadController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileAuthorizationService _fileAuthorizationService;
    private static readonly FormOptions _defaultFormOptions = new();

    public FileUploadController(
        SignInManager<ApplicationUser> signInManager,
        IFileUploadService fileUploadService,
        IFileAuthorizationService fileAuthorizationService)
    {
        _signInManager = signInManager;
        _fileUploadService = fileUploadService;
        _fileAuthorizationService = fileAuthorizationService;
    }

    [HttpPost("{scope}")]
    [Consumes("multipart/form-data")]
    [DisableFormValueModelBinding]
    [DisableRequestSizeLimit]
    [UploadFile(AllowMultiple = true, Description = "Upload one or more files to the specified scope using multipart/form-data.", Required = true)]
    public async Task<ActionResult<IList<FileUploadResult>>> UploadFiles([FromRoute] string scope)
    {
        var options = await _fileUploadService.GetOptionsAsync(scope);

        if (options is null)
        {
            return new[] { FileUploadError.InvalidScope(scope) };
        }

        var userId = GetUserId(await GetCurrentUser());

        if (!options.AllowAnonymousUpload && userId == AnonymousUser.UserName)
        {
            // Forbid() redirects to /Account/AccessDenied when there is no authorization header in the request.
            return StatusCode(StatusCodes.Status403Forbidden);
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
                request.UserId = userId;
                request.FileName = fileName;
                request.Stream = stream;

                var result = await _fileUploadService.UploadFileAsync(request);

                if (result.Url != null)
                {
                    // Hide real URL
                    result.Url = result.PublicUrl;
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
        var user = await GetCurrentUser();
        if (user is null)
        {
            return Forbid();
        }

        var file = await _fileUploadService.GetNoCloneAsync(id);
        if (file is null)
        {
            return NotFound();
        }

        var authorizationResult = await _fileAuthorizationService.AuthorizeAsync(user, file, FilePermissions.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        Stream stream;

        try
        {
            stream = await _fileUploadService.OpenReadAsync(id);
        }
        catch
        {
            stream = null;
        }

        return stream is null
            ? NotFound()
            : File(stream, file.ContentType, file.Name);
    }



    private async Task<ClaimsPrincipal> GetCurrentUser()
    {
        var principal = User;

        // Temporary workaround for requests from the storefront. Delete after getting rid of the storefront.
        if (Request.Headers.TryGetValue("VirtoCommerce-User-Name", out var userNameFromHeader) &&
            principal.IsInRole(PlatformConstants.Security.SystemRoles.Administrator))
        {
            principal = null;

            if (userNameFromHeader != AnonymousUser.UserName)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(userNameFromHeader);
                if (user != null)
                {
                    principal = await _signInManager.CreateUserPrincipalAsync(user);
                }
            }
        }

        return principal;
    }

    private static string GetUserId(ClaimsPrincipal user)
    {
        return user?.GetUserId() ?? AnonymousUser.UserName;
    }
}
