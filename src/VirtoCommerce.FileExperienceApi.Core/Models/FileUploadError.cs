using System;
using System.Collections.Generic;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public static class FileUploadError
{
    public static FileUploadResult InvalidContentType(string contentType)
    {
        return FileUploadResult.Fail("INVALID_CONTENT_TYPE", $"Expected a multipart request, but got '{contentType}'", contentType);
    }

    public static FileUploadResult InvalidContent()
    {
        return FileUploadResult.Fail("INVALID_CONTENT", "Cannot read file");
    }

    public static FileUploadResult InvalidScope(string scope)
    {
        return FileUploadResult.Fail("INVALID_SCOPE", $"Unknown scope '{scope}'", scope);
    }

    public static FileUploadResult InvalidExtension(IList<string> allowedExtensions)
    {
        var joinedExtensions = string.Join(", ", allowedExtensions);
        return FileUploadResult.Fail("INVALID_EXTENSION", $"Allowed file extensions: {joinedExtensions}", allowedExtensions);
    }

    public static FileUploadResult InvalidSize(long maxSize)
    {
        return FileUploadResult.Fail("INVALID_SIZE", $"Maximum allowed file size: {maxSize}", maxSize);
    }

    public static FileUploadResult Exception(Exception ex)
    {
        return FileUploadResult.Fail("EXCEPTION", ex.Message);
    }
}
