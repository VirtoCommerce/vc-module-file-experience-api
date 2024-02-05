using System.IO;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadRequest
{
    public string Scope { get; set; }
    public string UserId { get; set; }
    public string FileName { get; set; }
    public Stream Stream { get; set; }
}
