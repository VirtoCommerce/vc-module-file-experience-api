using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.FileExperienceApi.Data.Authorization;

public class FileAuthorizationRequirement : PermissionAuthorizationRequirement
{
    public FileAuthorizationRequirement(string permission)
        : base(permission)
    {
    }
}

public class FileAuthorizationHandler : PermissionAuthorizationHandlerBase<FileAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FileAuthorizationRequirement requirement)
    {
        var authorized = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

        if (!authorized && context.Resource is File file)
        {
            // Authorize only if the file is not attached to any entity.
            // Attached files should be processed by a different handler.
            authorized = string.IsNullOrEmpty(file.OwnerEntityId);
        }

        if (authorized)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
