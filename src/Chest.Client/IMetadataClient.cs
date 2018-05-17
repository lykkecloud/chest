// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Client
{
    using System;
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
        [Obsolete("Use generic method Add<T> instead")]
        Task Add(Metadata data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the specified instance as metadata against the given key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata</typeparam>
        /// <param name="key">Unique key for which to store metadata</param>
        /// <param name="instance">The instance object of type T</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task Add<T>(string key, T instance, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        [Obsolete("Use Generic method Get<T> instead")]
        Task<Metadata> GetMetadata(string key, CancellationToken cancellationToken = default);

#pragma warning disable CA1716 // Identifiers should not match keywords
        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <typeparam name="T">The type for which the metada was saved</typeparam>
        /// <param name="key">Unique key for which to get metadata</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key</returns>
        Task<T> Get<T>(string key, CancellationToken cancellationToken = default)
            where T : class, new();
#pragma warning restore CA1716
    }
}
