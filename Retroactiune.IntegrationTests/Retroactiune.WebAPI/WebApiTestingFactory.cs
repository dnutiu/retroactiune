using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

// ReSharper disable ClassNeverInstantiated.Global

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI
{
    /// <summary>
    /// Custom WebApplicationFactory used in integration testing, it uses TestingStartup for testing instead of Startup.
    /// </summary>
    public class WebApiTestingFactory : WebApplicationFactory<TestingStartup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestingStartup>();
            });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(".");
            base.ConfigureWebHost(builder);
        }
    }
}