// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Dto;

    /// <summary>
    /// Exposes public members of Metadata client
    /// </summary>
    public class MetadataClient : HttpClientBase, IMetadataClient
    {
        private const string ApiPath = "/api/metadata";

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataClient"/> class.
        /// </summary>
        /// <param name="service">The service url</param>
        /// <param name="innerHandler">The inner handler</param>
        public MetadataClient(string service, HttpMessageHandler innerHandler = null)
            : base(service, innerHandler)
        {
        }

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
            this.GetAsync<Metadata>(this.RelativeUrl($"{ApiPath}/{key}"), cancellationToken);
    }
}
