using Downcast.SecurityCodes.API.Controllers;
using Downcast.SecurityCodes.Repository.Options;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Downcast.SecurityCodes.Tests.Utils;

public class SecurityCodesServerInstance : WebApplicationFactory<SecurityCodesController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.Configure<RepositoryOptions>(options =>
            {
                options.Collection = Constants.CollectionName;
                options.ProjectId  = ProjectId;
            });
        });
    }

    private static string ProjectId => Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID") ??
                                       throw new InvalidOperationException(
                                           "FIRESTORE_PROJECT_ID environment variable is not set");
}