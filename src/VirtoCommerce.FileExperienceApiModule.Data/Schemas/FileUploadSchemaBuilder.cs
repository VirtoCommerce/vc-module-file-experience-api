using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.FileExperienceApiModule.Core;
using VirtoCommerce.FileExperienceApiModule.Core.Queries;

namespace VirtoCommerce.FileExperienceApiModule.Data.Schemas
{
    public class FileUploadSchemaBuilder : ISchemaBuilder
    {
        private readonly IMediator _mediator;

        public FileUploadSchemaBuilder(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            schema.Query.AddField(new FieldType
            {
                Name = "files",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scope" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "objectId" }
                ),
                Type = typeof(ListGraphType<FileGraphType>),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetFilesQuery
                    {
                        Scope = context.GetArgument<string>("scope"),
                        ObjectId = context.GetArgument<string>("objectId"),
                    });

                    return result.Files;
                })
            });

            //schema.Mutation?.AddField(FieldBuilder.Create<UploadGraphType, FileGraphType>()
            //    .Name("uploadFile")
            //    .Argument<NonNullGraphType<UploadGraphType>>("file", "File to upload")
            //    .ResolveAsync(async context =>
            //    {
            //        var file = context.GetArgument<IFormFile>("file");
            //        var result = await _mediator.Send(new GraphQLUploadFileMap { File = file });
            //        return null;
            //    })
            //    .Description("Upload file")
            //    .FieldType);
        }
    }
}
