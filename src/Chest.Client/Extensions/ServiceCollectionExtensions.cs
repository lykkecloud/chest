using System;
using Chest.Client.AutorestClient;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Chest.Client.Extensions
{
    /// <summary>
    /// Contains extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IChestClient"/> in service collection.
        /// </summary>
        /// <param name="services">The collection of service descriptors.</param>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="apiKey">The authentication API key.</param>
        /// <param name="timeout">The request timeout in second.</param>
        public static void AddChestClient(this IServiceCollection services, string serviceUrl, string apiKey = null,
            int? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            var builder = HttpClientGenerator
                .BuildForUrl(serviceUrl);

            if (timeout.HasValue)
                builder.WithTimeout(TimeSpan.FromSeconds(timeout.Value));

            if (!string.IsNullOrEmpty(apiKey))
                builder.WithApiKey(apiKey);

            var clientGenerator = builder.Create();

            services.AddSingleton<IChestClient>(provider => new ChestClientAdapter(clientGenerator));
        }
    }
}