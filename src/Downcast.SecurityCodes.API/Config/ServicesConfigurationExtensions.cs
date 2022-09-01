using Downcast.SecurityCodes.Manager;
using Downcast.SecurityCodes.Repository;
using Downcast.SecurityCodes.Repository.Options;

using Google.Api.Gax;
using Google.Cloud.Firestore;

using Mapster;

using MapsterMapper;

using Microsoft.Extensions.Options;

namespace Downcast.SecurityCodes.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder AddSecurityCodesApiServices(this WebApplicationBuilder builder)
    {
        builder.AddMapster();
        builder.Services.AddSingleton<ISecurityCodesManager, SecurityCodesManager>();
        
        builder.Services
            .AddOptions<SecurityCodesOptions>()
            .Bind(builder.Configuration.GetSection(SecurityCodesOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<ISecurityCodesRepository, SecurityCodesFirestoreRepository>();

        builder.AddRepository();

        return builder;
    }

    private static void AddRepository(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<RepositoryOptions>()
            .Bind(builder.Configuration.GetSection(RepositoryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton(provider =>
        {
            IOptions<RepositoryOptions> options = provider.GetRequiredService<IOptions<RepositoryOptions>>();
            return new FirestoreDbBuilder
            {
                ProjectId         = options.Value.ProjectId,
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();
        });
    }

    private static WebApplicationBuilder AddMapster(this WebApplicationBuilder builder)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IMapper, ServiceMapper>();
        return builder;
    }
}