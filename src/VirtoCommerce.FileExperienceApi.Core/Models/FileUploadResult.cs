using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class FileUploadResult : File
{
    public bool Succeeded => string.IsNullOrEmpty(ErrorMessage);
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorParameter { get; set; }

    public static FileUploadResult Success(File file)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();

        result.Scope = file.Scope;
        result.Id = file.Id;
        result.Name = file.Name;
        result.ContentType = file.ContentType;
        result.Size = file.Size;
        result.Url = file.Url;

        return result;
    }

    public static FileUploadResult Fail(string code, string message, object parameter = null, string fileName = null)
    {
        var result = AbstractTypeFactory<FileUploadResult>.TryCreateInstance();

        result.ErrorCode = code;
        result.ErrorMessage = message;
        result.ErrorParameter = parameter;
        result.Name = fileName;

        return result;
    }
}
