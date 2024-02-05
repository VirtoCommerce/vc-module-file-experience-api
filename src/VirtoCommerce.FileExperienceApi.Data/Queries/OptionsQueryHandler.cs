using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;

namespace VirtoCommerce.FileExperienceApi.Data.Queries;

public class OptionsQueryHandler : IQueryHandler<OptionsQuery, FileUploadScopeOptions>
{
    private readonly IFileUploadService _fileUploadService;

    public OptionsQueryHandler(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    public Task<FileUploadScopeOptions> Handle(OptionsQuery request, CancellationToken cancellationToken)
    {
        return _fileUploadService.GetOptionsAsync(request.Scope);
    }
}
