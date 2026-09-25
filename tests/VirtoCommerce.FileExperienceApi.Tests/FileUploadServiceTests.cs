using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Data.Services;
using Xunit;

namespace VirtoCommerce.FileExperienceApi.Tests
{
    public class FileUploadServiceTests
    {
        private const string Scope = "test-scope";
        private const string FileName = "document.pdf";

        [Fact]
        public async Task UploadFile_WhenEmptyFileAndMinFileSizeConfigured_ExpectRejectedAndBlobRemoved()
        {
            // Arrange
            var blobProvider = new FakeBlobStorageProvider();
            var service = CreateService(blobProvider, new FileUploadScopeOptions
            {
                Scope = Scope,
                MinFileSize = 1,
                MaxFileSize = 1000,
            });

            // Act
            var result = await service.UploadFileAsync(CreateRequest(size: 0));

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("INVALID_MIN_SIZE", result.ErrorCode);
            Assert.Equal(1L, result.ErrorParameter);
            Assert.Equal(FileName, result.Name);
            Assert.Contains("1", result.ErrorMessage);
            Assert.Single(blobProvider.RemovedUrls);
            Assert.Equal(blobProvider.WrittenUrls.Single(), blobProvider.RemovedUrls.Single());
        }

        [Fact]
        public async Task UploadFile_WhenFileAboveMinFileSize_ExpectSuccess()
        {
            // Arrange
            var blobProvider = new FakeBlobStorageProvider();
            var service = CreateService(blobProvider, new FileUploadScopeOptions
            {
                Scope = Scope,
                MinFileSize = 1,
                MaxFileSize = 1000,
            });

            // Act
            var result = await service.UploadFileAsync(CreateRequest(size: 10));

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.ErrorCode);
            Assert.Equal(10, result.Size);
            Assert.Empty(blobProvider.RemovedUrls);
        }

        [Fact]
        public async Task UploadFile_WhenEmptyFileAndMinFileSizeNotConfigured_ExpectSuccess()
        {
            // Arrange
            var blobProvider = new FakeBlobStorageProvider();
            var service = CreateService(blobProvider, new FileUploadScopeOptions
            {
                Scope = Scope,
                MaxFileSize = 1000,
            });

            // Act
            var result = await service.UploadFileAsync(CreateRequest(size: 0));

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.ErrorCode);
            Assert.Equal(0, result.Size);
            Assert.Empty(blobProvider.RemovedUrls);
        }

        [Fact]
        public async Task UploadFile_WhenFileAboveMaxFileSize_ExpectInvalidSize()
        {
            // Arrange
            var blobProvider = new FakeBlobStorageProvider();
            var service = CreateService(blobProvider, new FileUploadScopeOptions
            {
                Scope = Scope,
                MaxFileSize = 5,
            });

            // Act
            var result = await service.UploadFileAsync(CreateRequest(size: 10));

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("INVALID_SIZE", result.ErrorCode);
            Assert.Equal(5L, result.ErrorParameter);
            Assert.Single(blobProvider.RemovedUrls);
        }


        private static FileUploadService CreateService(FakeBlobStorageProvider blobProvider, FileUploadScopeOptions scopeOptions)
        {
            var options = new FileUploadOptions();
            options.Scopes.Add(scopeOptions);

            return new FileUploadService(
                new FakeFileExtensionService(),
                Options.Create(options),
                new FakeAssetEntryService(),
                blobProvider);
        }

        private static FileUploadRequest CreateRequest(int size)
        {
            return new FileUploadRequest
            {
                Scope = Scope,
                UserId = "user1",
                FileName = FileName,
                Stream = new MemoryStream(new byte[size]),
            };
        }

        private sealed class FakeBlobStorageProvider : IBlobStorageProvider
        {
            public IList<string> WrittenUrls { get; } = new List<string>();
            public IList<string> RemovedUrls { get; } = new List<string>();

            public Task<Stream> OpenWriteAsync(string blobUrl)
            {
                WrittenUrls.Add(blobUrl);
                return Task.FromResult<Stream>(new MemoryStream());
            }

            public Task RemoveAsync(string[] urls)
            {
                foreach (var url in urls)
                {
                    RemovedUrls.Add(url);
                }

                return Task.CompletedTask;
            }

            public Stream OpenWrite(string blobUrl) => throw new NotSupportedException();
            public Stream OpenRead(string blobUrl) => throw new NotSupportedException();
            public Task<Stream> OpenReadAsync(string blobUrl) => throw new NotSupportedException();
            public Task<BlobEntrySearchResult> SearchAsync(string folderUrl, string keyword) => throw new NotSupportedException();
            public Task<BlobInfo> GetBlobInfoAsync(string blobUrl) => throw new NotSupportedException();
            public Task CreateFolderAsync(BlobFolder folder) => throw new NotSupportedException();
            public void Move(string srcUrl, string dstUrl) => throw new NotSupportedException();
            public Task MoveAsyncPublic(string srcUrl, string dstUrl) => throw new NotSupportedException();
            public void Copy(string srcUrl, string dstUrl) => throw new NotSupportedException();
            public Task CopyAsync(string srcUrl, string dstUrl) => throw new NotSupportedException();
        }

        private sealed class FakeAssetEntryService : IAssetEntryService
        {
            public IList<AssetEntry> Saved { get; } = new List<AssetEntry>();

            public Task<IList<AssetEntry>> GetAsync(IList<string> ids, string responseGroup = null, bool clone = true)
            {
                return Task.FromResult<IList<AssetEntry>>(Saved.Where(x => ids.Contains(x.Id)).ToList());
            }

            public Task SaveChangesAsync(IList<AssetEntry> models)
            {
                foreach (var model in models)
                {
                    Saved.Add(model);
                }

                return Task.CompletedTask;
            }

            public Task DeleteAsync(IList<string> ids, bool softDelete = false) => Task.CompletedTask;
        }

        private sealed class FakeFileExtensionService : IFileExtensionService
        {
            public Task<IList<string>> GetWhiteListAsync() => Task.FromResult<IList<string>>(new List<string>());
            public Task<IList<string>> GetBlackListAsync() => Task.FromResult<IList<string>>(new List<string>());
            public Task<bool> IsExtensionAllowedAsync(string extension) => Task.FromResult(true);
        }
    }
}
