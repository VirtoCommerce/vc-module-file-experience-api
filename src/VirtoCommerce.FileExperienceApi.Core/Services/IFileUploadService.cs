using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.FileExperienceApi.Core.Services
{
    public interface IFileUploadService
    {
        FileUploadScopeOptions GetOptions(string scope);
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task<IList<File>> GetFilesAsync(IList<string> ids);
        Task<File> OpenReadAsync(string id);
        Task DeleteFilesAsync(IList<string> ids);
    }
}
