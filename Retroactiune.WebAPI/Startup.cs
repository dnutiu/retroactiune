using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Retroactiune.Services;
using Retroactiune.Settings;

namespace Retroactiune
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database Configuration
            services.Configure<RetroactiuneDbSettings>(Configuration.GetSection(nameof(RetroactiuneDbSettings)));
            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<RetroactiuneDbSettings>>().Value);

            // AutoMapper
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
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retroactiune API");
                c.RoutePrefix = "";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            logger.LogInformation("Running");
        }
    }
}