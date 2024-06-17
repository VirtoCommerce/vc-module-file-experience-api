using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.FileExperienceApi.Data.Queries;

public class OptionsQuery : Query<FileUploadScopeOptions>
{
    public string Scope { get; set; }


    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<StringGraphType>(nameof(Scope));
    }

    public override void Map(IResolveFieldContext context)
    {
        Scope = context.GetArgument<string>(nameof(Scope));
    }
}
