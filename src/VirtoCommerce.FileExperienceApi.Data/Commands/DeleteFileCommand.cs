using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.FileExperienceApi.Data.Commands;

public class DeleteFileCommand : ICommand<bool>
{
    public string Id { get; set; }
}

public class DeleteFileCommandType : InputObjectGraphType<DeleteFileCommand>
{
    public DeleteFileCommandType()
    {
        Field(x => x.Id);
    }
}
