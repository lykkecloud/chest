// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using System.Linq;
    using CacheManager.Core;
    using Chest.Data;
    using Chest.Mappers;
    using Chest.Services;
    using EFSecondLevelCache.Core;
    using Lykke.Middlewares;
    using Lykke.Middlewares.Mappers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ApiVersion => "v2";

        public string ApiTitle => "Chest API";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(this.configuration.GetConnectionString("Chest")));

            services
                .AddMvc()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
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
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            // Configure swagger
            services.AddSwaggerGen(options =>
            {
                // Specifying versions
                options.SwaggerDoc("v1", this.CreateInfoForApiVersion("v1", true));
                options.SwaggerDoc("v2", this.CreateInfoForApiVersion("v2", false));

                options.SchemaFilter<NullableTypeSchemaFilter>();

                // This call remove version from parameter, without it we will have version as parameter for all endpoints in swagger UI
                options.OperationFilter<RemoveVersionFromParameter>();

                // This make replacement of v{version:apiVersion} to real version of corresponding swagger doc, i.e. v1
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                // This exclude endpoint not specified in swagger version, i.e. MapToApiVersion("99")
                options.DocInclusionPredicate((version, desc) =>
                {
                    var versions = desc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var maps = desc.ActionAttributes()
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToArray();

                    return versions.Any(v => $"v{v.ToString()}" == version) && (maps.Length == 0 || maps.Any(v => $"v{v.ToString()}" == version));
                });
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
            
            app.UseMiddleware<LogHandlerMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // app.UseCors("spa");
            app.UseMvcWithDefaultRoute();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint($"/swagger/v2/swagger.json", $"{this.ApiTitle} v2");
                x.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{this.ApiTitle} v1");
                x.EnableValidator(null);
            });

            app.InitializeDatabase();
        }

        public Info CreateInfoForApiVersion(string apiVersion, bool isObsolete)
        {
            var info = new Info()
            {
                Title = $"{this.ApiTitle}",
                Version = apiVersion,
                Description = this.ApiTitle,
                Contact = new Contact(),
                TermsOfService = "Copyright (c) Lykke Corp. See the LICENSE file in the project root for more information.",
                License = new License() { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            if (isObsolete)
            {
                info.Description += ". This API version is obsolete and will be discontinued soon.";
            }

            return info;
        }
    }
}
