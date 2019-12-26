// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Chest.Client.Models;
using JetBrains.Annotations;
using Refit;

namespace Chest.Client.Api
{
    /// <summary>
    /// The metadata operation contract.
    /// </summary>
    [PublicAPI]
    public interface IMetadata
    {
        [Get("/api/v2")]
        Task<List<string>> GetCategoriesAsync();

        [Get("/api/v2/{category}")]
        Task<List<string>> GetCollectionsAsync(string category);

        [Get("/api/v2/{category}/{collection}")]
        Task<Dictionary<string, string>> GetKeysWithDataAsync(string category, string collection,
            [Query] string keyword);

        [Get("/api/v2/{category}/{collection}/{key}")]
        Task<MetadataModelContract> GetAsync(string category, string collection, string key);

        [Post("/api/v2/{category}/{collection}/{key}")]
        Task CreateAsync(string category, string collection, string key, [Body] MetadataModelContract model);

        [Post("/api/v2/{category}/{collection}")]
        Task BulkCreateAsync(string category, string collection,
            [Body] Dictionary<string, MetadataModelContract> model);

        [Post("/api/v2/{category}/{collection}/find")]
        Task<IDictionary<string, string>> FindByKeysAsync(string category, string collection,
            [Body, Required] HashSet<string> keys, [Query] string keyword);

        [Put("/api/v2/{category}/{collection}/{key}")]
        Task<object> UpdateAsync(string category, string collection, string key, [Body] MetadataModelContract model);

        [Patch("/api/v2/{category}/{collection}")]
        Task BulkUpdateAsync(string category, string collection,
            [Body, Required] Dictionary<string, MetadataModelContract> model);

        [Delete("/api/v2/{category}/{collection}/{key}")]
        Task<object> DeleteAsync(string category, string collection, string key);

        [Delete("/api/v2/{category}/{collection}")]
        Task<object> BulkDeleteAsync(string category, string collection, [Body] HashSet<string> keys);
    }
}