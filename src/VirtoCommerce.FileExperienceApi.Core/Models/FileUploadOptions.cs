using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadOptions
{
    [Required]
    public string RootPath { get; set; }

    [Required]
    public List<FileUploadScopeOptions> Scopes { get; set; } = new();
}
