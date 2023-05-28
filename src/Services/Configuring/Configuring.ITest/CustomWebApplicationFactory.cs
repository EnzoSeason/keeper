using Configuring.Infrastructure.Settings;
using ITest.SeedWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Configuring.ITest;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> 
    where TProgram : class
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