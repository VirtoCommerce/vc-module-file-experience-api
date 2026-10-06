using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Extensions;

public static class FileUploadServiceExtensions
{
    public const string PublicUrlPrefix = "/api/files/";

    public static async Task SetOwnerAsync<T>(this IFileUploadService service, IList<string> urls, string scope, T owner)
        where T : IEntity
    {
        if (urls.IsNullOrEmpty())
        {
            return;
        }

        var files = (await service.GetByPublicUrlAsync(urls))
            .Where(x => x.Scope.EqualsIgnoreCase(scope))
            .ToList();

        if (files.Count == 0)
        {
            return;
        }

        foreach (var file in files)
        {
            file.SetOwner(owner);
        }

        await service.SaveChangesAsync(files);
    }

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
