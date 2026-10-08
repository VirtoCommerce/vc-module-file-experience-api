using System;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Authorization
{
    public interface IFileAuthorizationRequirementFactory
    {
        [Obsolete("Use CanCreateRequirement(File file) instead.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
        string Scope => null;

        bool CanCreateRequirement(File file)
        {
#pragma warning disable VC0015 // Type or member is obsolete
            return file.Scope.EqualsIgnoreCase(Scope);
#pragma warning restore VC0015 // Type or member is obsolete
        }

        IAuthorizationRequirement Create(File file, string permission);
    }
}
