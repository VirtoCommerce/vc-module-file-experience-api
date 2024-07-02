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
    private readonly StringComparer _ignoreCase = StringComparer.OrdinalIgnoreCase;

    private readonly FileUploadOptions _options;
    private readonly IFileExtensionService _fileExtensionService;
    private readonly IAssetEntryService _assetEntryService;
    private readonly IBlobStorageProvider _blobProvider;

    public FileUploadService(
        IFileExtensionService fileExtensionService,
        IOptions<FileUploadOptions> options,
        IAssetEntryService assetEntryService,
        IBlobStorageProvider blobProvider)
    {
        _options = options.Value;
        _fileExtensionService = fileExtensionService;
        _assetEntryService = assetEntryService;
        _blobProvider = blobProvider;
    }

    public virtual async Task<FileUploadScopeOptions> GetOptionsAsync(string scope)
    {
        var options = GetOptions(scope);
        if (options == null)
        {
            return null;
        }

        return new FileUploadScopeOptions
        {
            Scope = options.Scope,
            MaxFileSize = options.MaxFileSize,
            AllowedExtensions = await GetEffectiveAllowedExtensionsAsync(options.AllowedExtensions),
            AllowAnonymousUpload = options.AllowAnonymousUpload,
        };
    }

    public virtual async Task<FileUploadResult> UploadFileAsync(FileUploadRequest request)
    {
        var options = GetOptions(request.Scope);
        if (options is null)
        {
            return FileUploadError.InvalidScope(request.Scope, request.FileName);
        }

        var fileExtension = Path.GetExtension(request.FileName);
        if (!await IsExtensionAllowedAsync(fileExtension, options.AllowedExtensions))
        {
            return FileUploadError.InvalidExtension(await GetEffectiveAllowedExtensionsAsync(options.AllowedExtensions), request.FileName);
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
            blobInfo.Size = targetStream.Position;
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


    protected virtual FileUploadScopeOptions GetOptions(string scope)
    {
        return _options.Scopes.FirstOrDefault(x => x.Scope.EqualsInvariant(scope));
    }

    protected virtual async Task<bool> IsExtensionAllowedAsync(string extension, IList<string> allowedExtensions)
    {
        if (allowedExtensions.IsNullOrEmpty())
        {
            return await _fileExtensionService.IsExtensionAllowedAsync(extension);
        }

        return allowedExtensions.Contains(extension, _ignoreCase) &&
               await _fileExtensionService.IsExtensionAllowedAsync(extension);
    }

    protected virtual async Task<IList<string>> GetEffectiveAllowedExtensionsAsync(IList<string> allowedExtensions)
    {
        IList<string> result;

        var whiteList = await _fileExtensionService.GetWhiteListAsync();

        if (allowedExtensions.IsNullOrEmpty())
        {
            result = whiteList.IsNullOrEmpty()
                ? Array.Empty<string>()
                : whiteList;
        }
        else
        {
            result = whiteList.IsNullOrEmpty()
                ? allowedExtensions.Except(await _fileExtensionService.GetBlackListAsync(), _ignoreCase).ToArray()
                : allowedExtensions.Intersect(whiteList, _ignoreCase).ToArray();
        }

        return result;
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
