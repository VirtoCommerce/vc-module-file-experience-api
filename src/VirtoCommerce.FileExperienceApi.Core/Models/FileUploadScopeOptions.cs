using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.FileExperienceApi.Core.Models
{
    public class FileUploadScopeOptions
    {
        [Required]
        public string Scope { get; set; }

        [Required]
        public long MaxFileSize { get; set; }

        [Required]
        public IList<string> AllowedExtensions { get; set; }
    }
}
