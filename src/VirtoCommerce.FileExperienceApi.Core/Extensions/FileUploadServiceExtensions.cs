using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;

namespace VirtoCommerce.FileExperienceApi.Core.Extensions;

public static class FileUploadServiceExtensions
{
    public const string PublicUrlPrefix = "/api/files/";

    public static Task<IList<File>> GetByPublicUrlAsync(this IFileUploadService service, IList<string> urls, string responseGroup = null, bool clone = true)
    {
        var ids = urls
            .Select(GetFileId)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        return service.GetAsync(ids, responseGroup, clone);
    }

    public static string GetFileId(string publicUrl)
    {
        return publicUrl != null && publicUrl.StartsWith(PublicUrlPrefix)
            ? publicUrl[PublicUrlPrefix.Length..]
            : null;
    }

    public static string GetPublicUrl(string fileId)
    {
        return $"{PublicUrlPrefix}{fileId}";
    }
}
