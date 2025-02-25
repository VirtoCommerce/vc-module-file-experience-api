using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using File = VirtoCommerce.FileExperienceApi.Core.Models.File;

namespace VirtoCommerce.FileExperienceApi.Core.Services
{
    public interface IFileUploadService : ICrudService<File>
    {
        Task<FileUploadScopeOptions> GetOptionsAsync(string scope);
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task<Stream> OpenReadAsync(string id);
        Task<IList<File>> GetFiles(IList<string> urls, string attachmentsUrlPrefix = null);
        T ConvertTo<T>(File file, Action<T, File> converter, string attachmentsUrlPrefix = null) where T : IHasUrl;
        Dictionary<string, File> FilesToScopeOwnerDictionary<T>(IList<File> files, string scope, T owner, string attachmentsUrlPrefix = null) where T : Entity
    }
}
