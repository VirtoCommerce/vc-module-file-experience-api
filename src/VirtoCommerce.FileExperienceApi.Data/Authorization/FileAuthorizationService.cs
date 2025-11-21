using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Data.Authorization;

public class FileAuthorizationService : IFileAuthorizationService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IEnumerable<IFileAuthorizationRequirementFactory> _requirementFactories;

    public FileAuthorizationService(
        IAuthorizationService authorizationService,
        IEnumerable<IFileAuthorizationRequirementFactory> requirementFactories)
    {
        _authorizationService = authorizationService;
        _requirementFactories = requirementFactories;
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, File file, string permission)
    {
        var requirementFactory = _requirementFactories.FirstOrDefault(x => x.Scope.EqualsIgnoreCase(file.Scope));
        var requirement = requirementFactory?.Create(file, permission) ?? new FileAuthorizationRequirement(permission);

        return _authorizationService.AuthorizeAsync(user, file, requirement);
    }
}
