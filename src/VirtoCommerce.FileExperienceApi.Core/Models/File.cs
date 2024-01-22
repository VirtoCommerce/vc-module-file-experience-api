using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class File : Entity
{
    public string Scope { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }
    public string OwnerId { get; set; }
    public string OwnerType { get; set; }
}
