using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.FileExperienceApiModule.Core;
using VirtoCommerce.FileExperienceApiModule.Data.Schemas;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.FileExperienceApiModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection services)
        {
            services.AddSchemaBuilder<FileUploadSchemaBuilder>();

            var graphQlBuilder = new CustomGraphQLBuilder(services);
            graphQlBuilder.AddGraphTypes(typeof(FileUploadAnchor));

            services.AddMediatR(typeof(FileUploadAnchor).Assembly);
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
