using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using FilePermissions = VirtoCommerce.FileExperienceApi.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.FileExperienceApi.Data.Commands;

public class DeleteFileCommandBuilder : CommandBuilder<DeleteFileCommand, bool, DeleteFileCommandType, BooleanGraphType>
{
    protected override string Name => "DeleteFile";

    private readonly IFileUploadService _fileUploadService;
    private readonly IFileAuthorizationService _fileAuthorizationService;

    public DeleteFileCommandBuilder(
        IMediator mediator,
        IAuthorizationService authorizationService,
        IFileUploadService fileUploadService,
        IFileAuthorizationService fileAuthorizationService)
        : base(mediator, authorizationService)
    {
        _fileUploadService = fileUploadService;
        _fileAuthorizationService = fileAuthorizationService;
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, DeleteFileCommand request)
    {
        await base.BeforeMediatorSend(context, request);

        var file = await _fileUploadService.GetNoCloneAsync(request.Id);
        if (file is null)
        {
            return;
        }

        var authorizationResult = await _fileAuthorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), file, FilePermissions.Delete);

        if (!authorizationResult.Succeeded)
        {
            throw context.IsAuthenticated()
                ? AuthorizationError.Forbidden()
                : AuthorizationError.AnonymousAccessDenied();
        }
    }
}
