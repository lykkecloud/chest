// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using Chest.Data;
    using Chest.Services;
    using Lykke.Common.ApiLibrary.Swagger;
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

        public string ApiVersion => "v1";
        public string ApiTitle => "Chest API";

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(this.configuration.GetConnectionString("Chest")));

            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddJsonFormatters()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    })
                .AddDataAnnotations();

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "Chest API");
                options.SchemaFilter<NullableTypeSchemaFilter>();
            });

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
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint($"/swagger/{this.ApiVersion}/swagger.json", $"{this.ApiTitle} {this.ApiVersion}");
                x.EnableValidator(null);
            });
            app.InitializeDatabase();
        }
    }
}
