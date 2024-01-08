using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.FileExperienceApiModule.Core.Models
{
    public class FileUploadModel
    {
        public string Scope { get; set; }
        public IFormFile[] Files { get; set; }
    }
}
