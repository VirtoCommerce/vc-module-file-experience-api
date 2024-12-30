using GraphQL;
using GraphQL.MicrosoftDI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.FileExperienceApi.Core;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.FileExperienceApi.Data;
using VirtoCommerce.FileExperienceApi.Data.Authorization;
using VirtoCommerce.FileExperienceApi.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Xapi.Core.Extensions;

namespace VirtoCommerce.FileExperienceApi.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        _ = new GraphQLBuilder(serviceCollection, builder =>
        {
            var assemblyMarker = typeof(AssemblyMarker);
            builder.AddGraphTypes(assemblyMarker.Assembly);
            serviceCollection.AddMediatR(assemblyMarker);
            serviceCollection.AddAutoMapper(assemblyMarker);
            serviceCollection.AddSchemaBuilders(assemblyMarker);
        });

        serviceCollection.AddOptions<FileUploadOptions>().Bind(Configuration.GetSection("FileUpload")).ValidateDataAnnotations();
        serviceCollection.AddSingleton<IFileUploadService, FileUploadService>();
        serviceCollection.AddSingleton<IFileAuthorizationService, FileAuthorizationService>();
        serviceCollection.AddSingleton<IAuthorizationHandler, FileAuthorizationHandler>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "FileExperienceApi", ModuleConstants.Security.Permissions.AllPermissions);
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
