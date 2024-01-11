using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.FileExperienceApi.Core.Services;

namespace VirtoCommerce.FileExperienceApi.Data.Commands;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
{
    private readonly IFileUploadService _fileUploadService;

    public DeleteFileCommandHandler(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    public async Task<bool> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        await _fileUploadService.DeleteFileAsync(request.Id);

        return true;
    }
}
