// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Dto;
    using Chest.Extensions;

    /// <summary>
    /// Exposes public members of Metadata client
    /// </summary>
    public class MetadataClient : HttpClientBase, IMetadataClient
    {
        private const string ApiPath = "/api/metadata";

#pragma warning disable CA1054 // Uri parameters should not be strings
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataClient"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service url</param>
        /// <param name="innerHandler">The inner handler</param>
        public MetadataClient(string serviceUrl, HttpMessageHandler innerHandler = null)
            : base(serviceUrl, innerHandler)
        {
        }
#pragma warning restore CA1054 // Uri parameters should not be strings

        /// <summary>
        /// Adds the specified metadata.
        /// </summary>
        /// <param name="data">The metadata to store.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task Add(Metadata data, CancellationToken cancellationToken = default) =>
            this.SendAsync(HttpMethod.Post, this.RelativeUrl($"{ApiPath}"), data, cancellationToken);

        /// <summary>
        /// Adds the specified instance as metadata against the given key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata</typeparam>
        /// <param name="key">Unique key for which to store metadata</param>
        /// <param name="instance">The instance object of type T</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task Add<T>(string key, T instance, CancellationToken cancellationToken = default)
            where T : class
        {
            var metadata = new Metadata
            {
                Key = key,
                Data = instance.ToMetadataDictionary()
            };

            return this.SendAsync(HttpMethod.Post, this.RelativeUrl($"{ApiPath}"), metadata, cancellationToken);
        }

        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task<Metadata> GetMetadata(string key, CancellationToken cancellationToken = default) =>
            this.GetAsync<Metadata>(this.RelativeUrl($"{ApiPath}/{NotNull(key, nameof(key))}"), cancellationToken);

        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <typeparam name="T">The type for which the metada was saved</typeparam>
        /// <param name="key">Unique key for which to get metadata</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key</returns>
        public async Task<T> Get<T>(string key, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var metadata = await this.GetAsync<Metadata>(this.RelativeUrl($"{ApiPath}/{NotNull(key, nameof(key))}"), cancellationToken);

            return metadata?.Data?.To<T>();
        }
    }
}
