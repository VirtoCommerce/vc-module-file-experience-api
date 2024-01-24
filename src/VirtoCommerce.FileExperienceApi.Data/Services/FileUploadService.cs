using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using File = VirtoCommerce.FileExperienceApi.Core.Models.File;
using UrlHelpers = VirtoCommerce.Platform.Core.Extensions.UrlHelperExtensions;

namespace VirtoCommerce.FileExperienceApi.Data.Services;

public class FileUploadService : IFileUploadService
{
    private readonly FileUploadOptions _options;
    private readonly IAssetEntryService _assetEntryService;
    private readonly IBlobStorageProvider _blobProvider;

    public FileUploadService(
        IOptions<FileUploadOptions> options,
        IAssetEntryService assetEntryService,
        IBlobStorageProvider blobProvider)
    {
        _options = options.Value;
        _assetEntryService = assetEntryService;
        _blobProvider = blobProvider;
    }

    public virtual FileUploadScopeOptions GetOptions(string scope)
    {
        return _options.Scopes.FirstOrDefault(x => x.Scope.EqualsInvariant(scope));
    }

    public virtual async Task<FileUploadResult> UploadFileAsync(FileUploadRequest request)
    {
        var options = GetOptions(request.Scope);
        if (options is null)
        {
            return FileUploadError.InvalidScope(request.Scope, request.FileName);
        }

        var fileExtension = Path.GetExtension(request.FileName);
        if (!options.AllowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return FileUploadError.InvalidExtension(options.AllowedExtensions, request.FileName);
        }

        var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
        blobInfo.Name = Path.GetFileName(request.FileName);
        blobInfo.ContentType = MimeTypeResolver.ResolveContentType(blobInfo.Name);

        // Internal URL: rootPath/scope/userId/newGuid.ext
        var internalFileName = Path.ChangeExtension(NewGuid(), fileExtension);
        blobInfo.RelativeUrl = BuildFileUrl(_options.RootPath, options.Scope, request.UserId, internalFileName);

        await using (var targetStream = await _blobProvider.OpenWriteAsync(blobInfo.RelativeUrl))
        {
            await request.Stream.CopyToAsync(targetStream);
            blobInfo.Size = targetStream.Length;
        }

        if (blobInfo.Size > options.MaxFileSize)
        {
            await _blobProvider.RemoveAsync(new[] { blobInfo.RelativeUrl });
            return FileUploadError.InvalidSize(options.MaxFileSize, request.FileName);
        }

        var asset = AbstractTypeFactory<AssetEntry>.TryCreateInstance();
        asset.Id = NewGuid();
        asset.Group = options.Scope;
        asset.BlobInfo = blobInfo;

        await _assetEntryService.SaveChangesAsync(new[] { asset });

        var file = ConvertToFile(asset);
        return FileUploadResult.Success(file);
    }

    public virtual async Task<Stream> OpenReadAsync(string id)
    {
        var asset = await _assetEntryService.GetNoCloneAsync(id);

        return asset is null
            ? null
            : await _blobProvider.OpenReadAsync(asset.BlobInfo.RelativeUrl);
    }

    public async Task<IList<File>> GetAsync(IList<string> ids, string responseGroup = null, bool clone = true)
    {
        var assets = await _assetEntryService.GetNoCloneAsync(ids);

        var files = assets.Select(ConvertToFile).ToList();
        return files;
    }

    public virtual async Task DeleteAsync(IList<string> ids, bool softDelete = false)
    {
        var assets = await _assetEntryService.GetNoCloneAsync(ids);

        if (assets.Any())
        {
            var existingIds = assets.Select(x => x.Id).ToArray();
            await _assetEntryService.DeleteAsync(existingIds);

            var existingUrls = assets.Select(x => x.BlobInfo.RelativeUrl).ToArray();
            await _blobProvider.RemoveAsync(existingUrls);
        }
    }

    public virtual Task SaveChangesAsync(IList<File> models)
    {
        var assets = models.Select(ConvertToAsset).ToList();
        return _assetEntryService.SaveChangesAsync(assets);
    }


    protected virtual string BuildFileUrl(params string[] parts)
    {
        string result = null;

        foreach (var (part, i) in parts.Select((x, i) => (x, i)))
        {
            result = i == 0
                ? part
                : UrlHelpers.Combine(result, part);
        }

        return result;
    }

    protected virtual File ConvertToFile(AssetEntry asset)
    {
        var result = AbstractTypeFactory<File>.TryCreateInstance();

        result.Id = asset.Id;
        result.Scope = asset.Group;

        if (asset.BlobInfo != null)
        {
            result.Name = asset.BlobInfo.Name;
            result.ContentType = asset.BlobInfo.ContentType;
            result.Size = asset.BlobInfo.Size;
            result.Url = asset.BlobInfo.RelativeUrl;
        }

        if (asset.Tenant != null)
        {
            result.OwnerEntityId = asset.Tenant.Id;
            result.OwnerEntityType = asset.Tenant.Type;
        }

        return result;
    }

    protected virtual AssetEntry ConvertToAsset(File file)
    {
        var result = AbstractTypeFactory<AssetEntry>.TryCreateInstance();

        result.Id = file.Id;
        result.Group = file.Scope;

        result.BlobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
        result.BlobInfo.Name = file.Name;
        result.BlobInfo.ContentType = file.ContentType;
        result.BlobInfo.Size = file.Size;
        result.BlobInfo.RelativeUrl = file.Url;

        if (!string.IsNullOrEmpty(file.OwnerEntityId) || !string.IsNullOrEmpty(file.OwnerEntityType))
        {
            result.Tenant = new TenantIdentity(file.OwnerEntityId, file.OwnerEntityType);
        }

        return result;
    }

    protected virtual string NewGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
}
