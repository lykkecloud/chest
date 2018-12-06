// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using CacheManager.Core;
    using Chest.Data;
    using Chest.Services;
    using EFSecondLevelCache.Core;
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

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ApiVersion => "v1";

        public string ApiTitle => "Chest API";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(this.configuration.GetConnectionString("Chest")));

            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });

            var cacheManagerConfiguration = new CacheManager.Core.ConfigurationBuilder()
                .WithJsonSerializer()
                .WithMicrosoftMemoryCacheHandle()
                .Build();

            services.AddEFSecondLevelCache();
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration), cacheManagerConfiguration);

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "Chest API");
                options.SchemaFilter<NullableTypeSchemaFilter>();
            });

            // Default settings for NewtonSoft Serializer
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

            services.AddScoped<IDataService, DataService>();

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
            app.UseEFSecondLevelCache();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
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
