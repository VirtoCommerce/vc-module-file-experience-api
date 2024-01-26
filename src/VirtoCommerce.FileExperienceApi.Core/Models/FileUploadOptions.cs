using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadOptions
{
    [Required]
    public string RootPath { get; set; } = "upload";

    [Required]
    public List<FileUploadScopeOptions> Scopes { get; set; } = new();
}
