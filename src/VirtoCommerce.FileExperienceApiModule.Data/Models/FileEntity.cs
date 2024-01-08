using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApiModule.Data.Models
{
    public class FileEntity : AuditableEntity
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string Scope { get; set; }
    }
}
