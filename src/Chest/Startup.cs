// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using Chest.Data;
    using Chest.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(this.configuration.GetConnectionString("Chest")));

            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    })
                .AddDataAnnotations();

            // Default settings for Newtonsoft Serializer
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                    NullValueHandling = NullValueHandling.Ignore,
                };

                settings.Converters.Add(new StringEnumConverter());

                return settings;
            };

            services.AddScoped<IMetadataService, MetadataService>();

            services.AddCors(options =>
            {
                /*//options.AddPolicy("spa", policy =>
                //{
                //    policy.WithOrigins("http://localhost:5008")
                //        .AllowAnyHeader()
                //        .AllowAnyMethod();
                //});*/
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            // app.UseCors("spa");
            app.UseMvcWithDefaultRoute();
            app.InitializeDatabase();
        }
    }
}
