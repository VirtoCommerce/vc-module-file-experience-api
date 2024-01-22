using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;
using File = VirtoCommerce.FileExperienceApi.Core.Models.File;

namespace VirtoCommerce.FileExperienceApi.Core.Services
{
    public interface IFileUploadService : ICrudService<File>
    {
        FileUploadScopeOptions GetOptions(string scope);
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task<Stream> OpenReadAsync(string id);
    }
}
