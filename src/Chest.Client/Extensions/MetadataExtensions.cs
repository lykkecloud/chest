// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Chest.Client.Api;
using Chest.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MoreLinq;
using Newtonsoft.Json;

namespace Chest.Client.Extensions
{
    /// <summary>
    /// The <see cref="IMetadata"/> generic extension methods.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class MetadataExtensions
    {
        /// <summary>
        /// Adds the specified instance as metadata against the given category, collection and key. 
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata.</typeparam>
        /// <param name='operations'>The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="key">Unique key for which to store metadata.</param>
        /// <param name="instance">The instance object of type T.</param>
        /// <param name="keywords">Keywords associated with the data, these keywords will be used to search the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task Add<T>(this IMetadata operations, string category, string collection, string key,
            T instance, List<string> keywords = null, CancellationToken cancellationToken = default)
            where T : class
        {
            var model = new MetadataModelContract
            {
                Data = JsonConvert.SerializeObject(instance.ToMetadataDictionary()),
                Keywords = keywords == null ? string.Empty : JsonConvert.SerializeObject(keywords),
            };

            await operations.CreateAsync(category, collection, key, model);
        }

        /// <summary>
        /// Adds a set of instances to a specified category and collection.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata.</typeparam>
        /// <param name='operations'>The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="data">A dictionary of keys and associated metadata instances and keywords.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task BulkAdd<T>(this IMetadata operations, string category, string collection,
            Dictionary<string, (T instance, List<string> keywords)> data, CancellationToken cancellationToken = default)
            where T : class
        {
            var serializedDict = data.ToDictionary(x => x.Key, x => new MetadataModelContract
            {
                Data = JsonConvert.SerializeObject(x.Value.instance.ToMetadataDictionary()),
                Keywords = x.Value.keywords == null ? string.Empty : JsonConvert.SerializeObject(x.Value.keywords),
            });

            await operations.BulkCreateAsync(category, collection, serializedDict);
        }

        /// <summary>
        /// Updates the specified instance as metadata against the given category, collection and key.
        /// </summary>
        /// <typeparam name="T">Type of the instance object to store as metadata.</typeparam>
        /// <param name='operations'>The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="key">Unique key for which to store metadata.</param>
        /// <param name="instance">The instance object of type T.</param>
        /// <param name="keywords">Keywords associated with the data, these keywords will be used to search the data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static async Task Update<T>(this IMetadata operations, string category, string collection, string key,
            T instance, List<string> keywords = null, CancellationToken cancellationToken = default)
            where T : class
        {
            var model = new MetadataModelContract
            {
                Data = JsonConvert.SerializeObject(instance.ToMetadataDictionary()),
                Keywords = keywords == null ? string.Empty : JsonConvert.SerializeObject(keywords),
            };

            await operations.UpdateAsync(category, collection, key, model);
        }

        /// <summary>
        /// Updates multiple sets of key value pairs in a given category and collection.
        /// </summary>
        /// <typeparam name="T">The object type representing the metadata.</typeparam>
        /// <param name="operations">The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> containing the keys to update the metadata and keywords for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task BulkUpdate<T>(this IMetadata operations, string category, string collection,
            Dictionary<string, (T metadata, List<string> keywords)> data, CancellationToken cancellationToken = default)
            where T : class
        {
            await operations.BulkUpdateAsync(category, collection, data.ToDictionary(x => x.Key,
                x => new MetadataModelContract
                {
                    Data = JsonConvert.SerializeObject(x.Value.metadata.ToMetadataDictionary()),
                    Keywords = x.Value.keywords == null ? string.Empty : JsonConvert.SerializeObject(x.Value.keywords)
                }));
        }

        /// <summary>
        /// Gets metadata for specified category, collection and key. 
        /// </summary>
        /// <typeparam name="T">The type for which the metadata was saved.</typeparam>
        /// <param name='operations'>The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="key">Unique key for which to get metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key</returns>
        public static async Task<T> Get<T>(this IMetadata operations, string category, string collection, string key,
            CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var (instance, _) = await operations.GetWithKeywords<T>(category, collection, key, cancellationToken);
            return instance;
        }

        /// <summary>
        /// Gets metadata for specified category, collection and key.
        /// </summary>
        /// <typeparam name="T">The type for which the metadata was saved.</typeparam>
        /// <param name='operations'>The operations group for this extension method.</param>
        /// <param name="category">The category.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="key">Unique key for which to get metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Instance object of the type T containing all the metadata information against the given key.</returns>
        public static async Task<(T instance, IList<string> keywords)> GetWithKeywords<T>(this IMetadata operations,
            string category, string collection, string key, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var metadata = await operations.GetAsync(category, collection, key);
            return (metadata?.Data?.To<IDictionary<string, string>>().To<T>(),
                string.IsNullOrWhiteSpace(metadata?.Keywords)
                    ? new List<string>()
                    : metadata.Keywords.To<IList<string>>());
        }

        /// <summary>
        /// Looks up a set of metadata using a set of keys to search for.
        /// </summary>
        /// <typeparam name="T">The type which the metadata will be deserialized to.</typeparam>
        /// <param name="operations">The operations group for this extension method.</param>
        /// <param name="category">The category to search in.</param>
        /// <param name="collection">The collection to search in.</param>
        /// <param name="keys">The set of keys to search for.</param>
        /// <param name="keyword">An optional keyword to narrow down the search.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>A dictionary of key and its metadata.</returns>
        public static async Task<IDictionary<string, T>> FindByKeys<T>(this IMetadata operations, string category,
            string collection, IList<string> keys, string keyword = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var metadataDict = await operations.FindByKeysAsync(category, collection, keys.ToHashSet(), keyword);
            return metadataDict.ToDictionary(x => x.Key, x => x.Value?.To<IDictionary<string, string>>().To<T>());
        }

        /// <summary>
        /// Gets all keys with their metadata in the system against the given category, and collection.
        /// </summary>
        /// <param name="operations">The operations group for this extension method.</param>
        /// <param name="category">The category for which to get keys.</param>
        /// <param name="collection">The collection for which to get keys in the category.</param>
        /// <param name="keyword">Optional param to search key value pairs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> having the keys and their metadata.</returns>
        public static async Task<IDictionary<string, T>> GetKeysWithData<T>(this IMetadata operations, string category,
            string collection, string keyword = null, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var list = await operations.GetKeysWithDataAsync(category, collection, keyword);
            return list?.ToDictionary(d => d.Key, d => d.Value?.To<IDictionary<string, string>>().To<T>());
        }
    }
}