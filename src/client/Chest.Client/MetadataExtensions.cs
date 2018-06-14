// Copyright 2017 Lykke Corp.
// See LICENSE file in the project root for full license information.

namespace Chest.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Client.AutorestClient;
    using Chest.Client.AutorestClient.Models;

    public static class MetadataExtensions
    {
        /// <summary>
        /// Adds the specified instance as metadata against the given category, collection and key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata</typeparam>
        /// <param name='operations'>The operations group for this extension method</typeparam>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">Unique key for which to store metadata</param>
        /// <param name="instance">The instance object of type T</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task Add<T>(this IMetadata operations, string category, string collection, string key, T instance, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var model = new MetadataModel
            {
                Data = instance.ToMetadataDictionary()
            };

            await operations.AddAsync(category, collection, key, model, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified instance as metadata against the given category, collection and key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata</typeparam>
        /// <param name='operations'>The operations group for this extension method</typeparam>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">Unique key for which to store metadata</param>
        /// <param name="instance">The instance object of type T</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task Update<T>(this IMetadata operations, string category, string collection, string key, T instance, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var model = new MetadataModel
            {
                Data = instance.ToMetadataDictionary()
            };

            await operations.UpdateAsync(category, collection, key, model, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets metadata for specified category, collection and key.
        /// </summary>
        /// <typeparam name="T">The type for which the metada was saved</typeparam>
        /// <param name='operations'>The operations group for this extension method</typeparam>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">Unique key for which to get metadata</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key</returns>
        public static async Task<T> GetMetadataAsync<T>(this IMetadata operations, string category, string collection, string key, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, new()
        {
            var metadata = await operations.GetMetadataAsync(category, collection, key, cancellationToken).ConfigureAwait(false);

            return metadata?.Data?.To<T>();
        }

        /// <summary>
        /// Gets all keys with their metadata in the system against the given category, and collection
        /// </summary>
        /// <param name="category">The category for which to get keys</param>
        /// <param name="collection">The collection for which to get keys in the category</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> having the keys and their metadata</returns>
        public static async Task<IDictionary<string, T>> GetKeysWithDataAsync<T>(this IMetadata operations, string category, string collection, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, new()
        {
            var list = await operations.GetKeysWithDataAsync(category, collection, cancellationToken).ConfigureAwait(false);

            return list
                .ToDictionary(d => d.Key, d => d.Value.To<T>());
        }
    }
}
