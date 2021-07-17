using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Prometheus;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Retroactiune.Infrastructure;
using Sentry.AspNetCore;

namespace Retroactiune
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        // TODO: External auth provider.
        // TODO: UI? 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database Configuration
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            // Services
            services.AddSingleton<IFeedbackReceiversService, FeedbackReceiversService>();
            services.AddSingleton<ITokensService, TokensService>();
            services.AddSingleton<IFeedbacksService, FeedbacksService>();
            services.AddSingleton<IMongoClient, MongoClient>(i =>
            {
                var settings = i.GetService<IOptions<DatabaseSettings>>();
                return new MongoClient(settings.Value.ConnectionString);
            });

            // WebAPI
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Retroactiune.WebAPI.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMetricServer();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retroactiune API");
                c.RoutePrefix = "";
            });

            app.UseHttpsRedirection();
            
            app.UseSentryTracing();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            logger.LogInformation("Running");
        }
    }
}