// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Common.Startup.ApiKey;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chest
{
    using CacheManager.Core;
    using Data;
    using Mappers;
    using Services;
    using EFSecondLevelCache.Core;
    using Lykke.Middlewares;
    using Lykke.Middlewares.Mappers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        private static string ApiTitle => "Chest API";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(this._configuration.GetConnectionString("Chest")));

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver
                            {NamingStrategy = new SnakeCaseNamingStrategy()};
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });

            services.AddSingleton<IHttpStatusCodeMapper, HttpStatusCodeMapper>();
            services.AddSingleton<ILogLevelMapper, DefaultLogLevelMapper>();

            var cacheManagerConfiguration = new CacheManager.Core.ConfigurationBuilder()
                .WithJsonSerializer()
                .WithMicrosoftMemoryCacheHandle()
                .Build();

            services.AddEFSecondLevelCache();
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration), cacheManagerConfiguration);

            // Configure versions
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(2, 0);
            });

            var clientSettings = this._configuration.GetSection("ChestClientSettings").Get<ClientSettings>();
            services.AddApiKeyAuth(clientSettings);

            // Configure swagger
            services.AddSwaggerGen(options =>
            {
                // Specifying versions
                options.SwaggerDoc("v2", this.CreateInfoForApiVersion("v2", false));

                // This call remove version from parameter, without it we will have version as parameter for all endpoints in swagger UI
                options.OperationFilter<RemoveVersionFromParameter>();

                // This make replacement of v{version:apiVersion} to real version of corresponding swagger doc, i.e. v1
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();
                
                // This exclude endpoint not specified in swagger version, i.e. MapToApiVersion("99")
                options.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out var methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var maps = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToArray();
                    
                    return versions.Any(v => $"v{v.ToString()}" == version) && (maps.Length == 0 || maps.Any(v => $"v{v.ToString()}" == version));
                });

                if (!string.IsNullOrWhiteSpace(clientSettings?.ApiKey))
                {
                    options.AddApiKeyAwareness();
                }
            });
            
            // Default settings for NewtonSoft Serializer
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None
                };

                settings.Converters.Add(new StringEnumConverter());

                return settings;
            };

            services.AddScoped<IDataService, DataService>();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
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
            
            app.UseMiddleware<LogHandlerMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());
            
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, req) => doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = $"{req.Scheme}://{req.Host.Value}"
                    }
                });
            });
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint($"/swagger/v2/swagger.json", $"{ApiTitle} v2");
                x.EnableValidator(null);
            });
            
            app.InitializeDatabase();
        }

        private OpenApiInfo CreateInfoForApiVersion(string apiVersion, bool isObsolete)
        {
            var info = new OpenApiInfo
            {
                Title = $"{ApiTitle}",
                Version = apiVersion,
                Description = ApiTitle,
                Contact = new OpenApiContact(),
                License = new OpenApiLicense {Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")}
            };

            if (isObsolete)
            {
                info.Description += ". This API version is obsolete and will be discontinued soon.";
            }

            return info;
        }
    }
}
