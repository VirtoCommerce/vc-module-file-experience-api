using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public interface IHasUrl : IEntity
{
    string Url { get; set; }
}
