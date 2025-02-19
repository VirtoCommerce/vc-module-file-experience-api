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

    public static FileUploadResult Exception(Exception ex, string fileName = null)
    {
        return FileUploadResult.Fail("EXCEPTION", ex.Message, parameter: null, fileName);
    }

    public static FileUploadResult InvalidScope(string scope, string fileName = null)
    {
        return FileUploadResult.Fail("INVALID_SCOPE", $"Unknown scope '{scope}'", scope, fileName);
    }

    public static FileUploadResult InvalidExtension(IList<string> allowedExtensions, string fileName)
    {
        var joinedExtensions = string.Join(", ", allowedExtensions);
        return FileUploadResult.Fail("INVALID_EXTENSION", $"Allowed file extensions: {joinedExtensions}", allowedExtensions, fileName);
    }

    public static FileUploadResult InvalidSize(long maxSize, string fileName)
    {
        return FileUploadResult.Fail("INVALID_SIZE", $"Maximum allowed file size: {maxSize}", maxSize, fileName);
    }
}
