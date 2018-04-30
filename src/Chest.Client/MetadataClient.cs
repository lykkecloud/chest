// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Dto;

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
        /// Gets metadata for specified key.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task<Metadata> GetMetadata(string key, CancellationToken cancellationToken = default) =>
            this.GetAsync<Metadata>(this.RelativeUrl($"{ApiPath}/{NotNull(key, nameof(key))}"), cancellationToken);
    }
}
