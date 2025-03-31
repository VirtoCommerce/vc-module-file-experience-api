using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.FileExperienceApi.Core.Authorization
{
    public interface IFileAuthorizationRequirementFactory
    {
        string Scope { get; }
        IAuthorizationRequirement Create(File file, string permission);
    }
}
