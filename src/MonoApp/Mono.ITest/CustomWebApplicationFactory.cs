using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mono.API;

namespace Mono.ITest;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<MongoDbSettings>(settings =>
            {
                settings.ConnectionString = MongoHelper.ConnectionString;
                settings.DatabaseName = MongoHelper.DatabaseName;
            });
        });
    }
}