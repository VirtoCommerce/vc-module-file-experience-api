using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadResult
{
    public bool Succeeded => string.IsNullOrEmpty(Error) && File != null;
    public string Error { get; set; }
    public File File { get; set; }

    public static FileUploadResult Success(File file)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();
        result.File = file;
        return result;
    }

    public static FileUploadResult Fail(string message)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();
        result.Error = message;
        return result;
    }
}
