namespace VirtoCommerce.FileExperienceApiModule.Data.Models
{
    public class FileEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string Scope { get; set; }
    }
}
