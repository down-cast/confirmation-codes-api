using Downcast.Common.HttpClient.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Downcast.SecurityCodes.Client.DependencyInjection;

public static class SecurityCodesClientConfigExtension
{
    public static IServiceCollection AddSecurityCodesClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSectionName = "SecurityCodesClient")
    {
        services.AddRefitClient<ISecurityCodesClient>()
            .ConfigureDowncastHttpClient(configuration, configurationSectionName);
        return services;
    }
}