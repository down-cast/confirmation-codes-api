using System.Reflection;

namespace Downcast.SecurityCodes.API.Config;

public static class SwaggerConfigExtensions
{
    private static string XmlCommentsFilePath => $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, XmlCommentsFilePath));
        });
        return services;
    }
}