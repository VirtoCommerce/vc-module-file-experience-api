using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.FileExperienceApi.Data.Schemas;

public class FileUploadScopeOptionsType : ExtendableGraphType<FileUploadScopeOptions>
{
    public FileUploadScopeOptionsType()
    {
        Field(x => x.Scope);
        Field(x => x.MaxFileSize);
        Field(x => x.AllowedExtensions);
    }
}
