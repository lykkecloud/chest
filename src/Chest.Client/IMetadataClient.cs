// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Dto;

    /// <summary>
    /// Exposes public members of Metadata client
    /// </summary>
    public interface IMetadataClient
    {
        /// <summary>
        /// Adds the specified metadata.
        /// </summary>
        /// <param name="data">The metadata to store.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task Add(Metadata data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task<Metadata> GetMetadata(string key, CancellationToken cancellationToken = default);
    }
}
