using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.FileExperienceApi.Data.Commands;

public class DeleteFileCommandBuilder : CommandBuilder<DeleteFileCommand, bool, DeleteFileCommandType, BooleanGraphType>
{
    protected override string Name => "DeleteFile";

    public DeleteFileCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }
}
