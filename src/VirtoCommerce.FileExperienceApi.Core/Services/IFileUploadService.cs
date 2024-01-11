using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.FileExperienceApi.Core.Services
{
    public interface IFileUploadService
    {
        FileUploadScopeOptions GetOptions(string scope);
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task DeleteFileAsync(string id);
        Task<File> OpenReadAsync(string id);
    }
}
