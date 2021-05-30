using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Retroactiune.Services;
using Retroactiune.Settings;

namespace Retroactiune
{
    public class TestingStartup
    {
        public TestingStartup(IConfiguration configuration)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Testing.json");

            Configuration = configurationBuilder.Build();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database Configuration
            services.Configure<RetroactiuneDbSettings>(Configuration.GetSection(nameof(RetroactiuneDbSettings)));
            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<RetroactiuneDbSettings>>().Value);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Services
            services.AddSingleton<IFeedbackReceiverService, FeedbackReceiverService>();
            services.AddSingleton<IMongoClient, MongoClient>(i =>
            {
                var settings = i.GetService<IOptions<RetroactiuneDbSettings>>();
                return new MongoClient(settings.Value.ConnectionString);
            });
            
            // WebAPI
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}