using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Data.Schemas;

namespace VirtoCommerce.FileExperienceApi.Data.Queries;

public class OptionsQueryBuilder : QueryBuilder<OptionsQuery, FileUploadScopeOptions, FileUploadScopeOptionsType>
{
    protected override string Name => "FileUploadOptions";

    public OptionsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }
}
