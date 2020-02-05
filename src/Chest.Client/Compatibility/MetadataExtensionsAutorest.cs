// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Chest.Client.AutorestClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Chest.Client.Extensions;
using MoreLinq;
using Newtonsoft.Json;
using Refit;

namespace Chest.Client.AutorestClient
{
    /// <summary> 
    /// Extension methods for Metadata. 
    /// </summary> 
    public static class MetadataExtensions
    {
        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='key'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task<MetadataModel> GetAsync(this IMetadata operations, string category,
            string collection, string key, CancellationToken cancellationToken = default)
        {
            MetadataModelContract metadata; 

            try
            {
                metadata = await operations.RefitClient.Get(category, collection, key);
            }
            catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(metadata.Data);
            var deserializedKeywords = string.IsNullOrWhiteSpace(metadata.Keywords)
                ? default
                : JsonConvert.DeserializeObject<List<string>>(metadata.Keywords);

            return new MetadataModel {Data = deserializedData, Keywords = deserializedKeywords};
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='key'> 
        /// </param> 
        /// <param name='model'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task UpdateAsync(this IMetadata operations, string category, string collection,
            string key, MetadataModel model = default, CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.Update(category, collection, key, model.ToChestContract());
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='key'> 
        /// </param> 
        /// <param name='model'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task AddAsync(this IMetadata operations, string category, string collection, string key,
            MetadataModel model = default, CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.Create(category, collection, key, model.ToChestContract());
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='key'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task RemoveAsync(this IMetadata operations, string category, string collection, string key,
            CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.Delete(category, collection, key);
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='keyword'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task<IDictionary<string, string>> GetKeysWithDataAsync(this IMetadata operations,
            string category, string collection, string keyword = default, CancellationToken cancellationToken = default)
        {
            try
            {
                return await operations.RefitClient.GetKeysWithData(category, collection, keyword);
            }
            catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='model'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task BulkAddAsync(this IMetadata operations, string category, string collection,
            IDictionary<string, MetadataModel> model = default, CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.BulkCreate(category, collection, model?.ToDictionary(x => x.Key,
                x => x.Value.ToChestContract()));
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='keys'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task BulkRemoveAsync(this IMetadata operations, string category, string collection,
            IList<string> keys = default, CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.BulkDelete(category, collection, keys.ToHashSet());
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='model'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task BulkUpdateAsync(this IMetadata operations, string category, string collection,
            IDictionary<string, MetadataModel> model = default, CancellationToken cancellationToken = default)
        {
            await operations.RefitClient.BulkUpdate(category, collection, model?.ToDictionary(x => x.Key,
                x => x.Value.ToChestContract()));
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task<IList<string>> GetCategoriesAsync(this IMetadata operations,
            CancellationToken cancellationToken = default)
        {
            return await operations.RefitClient.GetCategories();
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task<IList<string>> GetCollectionsAsync(this IMetadata operations, string category,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await operations.RefitClient.GetCollections(category);
            }
            catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <param name='operations'> 
        /// The operations group for this extension method. 
        /// </param> 
        /// <param name='category'> 
        /// </param> 
        /// <param name='collection'> 
        /// </param> 
        /// <param name='keys'> 
        /// </param> 
        /// <param name='keyword'> 
        /// </param> 
        /// <param name='cancellationToken'> 
        /// The cancellation token. 
        /// </param> 
        public static async Task<IDictionary<string, string>> FindByKeysAsync(this IMetadata operations,
            string category, string collection, IList<string> keys = default, string keyword = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await operations.RefitClient.FindByKeys(category, collection, keys.ToHashSet(), keyword);
            }
            catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}