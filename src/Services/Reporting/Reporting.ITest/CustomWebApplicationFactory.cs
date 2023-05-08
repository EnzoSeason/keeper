using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Reporting.Infrastructure.Settings;

namespace Reporting.ITest;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<MongoDbSettings>(settings =>
            {
                settings.ConnectionString = MongoDbTestInstance.ConnectionString;
                settings.DatabaseName = MongoDbTestInstance.DatabaseName;
            });
        });
    }
}