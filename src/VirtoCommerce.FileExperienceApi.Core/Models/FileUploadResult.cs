using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadResult
{
    public bool Succeeded => string.IsNullOrEmpty(ErrorMessage) && File != null;
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorParameter { get; set; }

    public File File { get; set; }

    public static FileUploadResult Success(File file)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();
        result.File = file;
        return result;
    }

    public static FileUploadResult Fail(string code, string message, object parameter = null, string fileName = null)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();

        result.ErrorCode = code;
        result.ErrorMessage = message;
        result.ErrorParameter = parameter;

        if (fileName != null)
        {
            result.File = AbstractTypeFactory<File>.TryCreateInstance();
            result.File.Name = fileName;
        }

        return result;
    }
}
