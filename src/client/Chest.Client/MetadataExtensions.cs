// Copyright 2017 Lykke Corp.
// See LICENSE file in the project root for full license information.

namespace Chest.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using Chest.Client.AutorestClient;
    using Chest.Client.AutorestClient.Models;

    public static class MetadataExtensions
    {
        /// <summary>
        /// Adds the specified instance as metadata against the given key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata</typeparam>
        /// <param name='operations'>The operations group for this extension method</typeparam>
        /// <param name="key">Unique key for which to store metadata</param>
        /// <param name="instance">The instance object of type T</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task AddAsync<T>(this IMetadata operations, string key, T instance, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var model = new MetadataModel
            {
                Key = key,
                Data = instance.ToMetadataDictionary()
            };

            await operations.AddAsync(model, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Gets metadata for specified key.
        /// </summary>
        /// <typeparam name="T">The type for which the metada was saved</typeparam>
        /// <param name='operations'>The operations group for this extension method</typeparam>
        /// <param name="key">Unique key for which to get metadata</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key</returns>
        public static async Task<T> GetAsync<T>(this IMetadata operations, string key, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, new()
        {
            var metadata = await operations.GetAsync(key, cancellationToken).ConfigureAwait(false);

            return metadata?.Data?.To<T>();
        }
    }
}
