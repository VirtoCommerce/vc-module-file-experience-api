using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;
using File = VirtoCommerce.FileExperienceApi.Core.Models.File;

namespace VirtoCommerce.FileExperienceApi.Core.Services
{
    public interface IFileUploadService : ICrudService<File>
    {
        Task<FileUploadScopeOptions> GetOptionsAsync(string scope);
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task<Stream> OpenReadAsync(string id);
        Task<IList<File>> GetByPublicUrlAsync(IList<string> urls, string responseGroup = null, bool clone = true);
        string GetFileId(string publicUrl);
        string GetPublicUrl(string fileId);
    }
}
