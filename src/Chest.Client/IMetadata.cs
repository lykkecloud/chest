// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Refit;

namespace Chest.Client
{
    public interface IMetadata
    {
        [Post("/api/v2/{category}/{collection}/{key}")]
        Task Create(string category, string collection, string key, [Body] MetadataModelContract model);

        [Post("/api/v2/{category}/{collection}")]
        Task BulkCreate(string category, string collection, [Body] Dictionary<string, MetadataModelContract> model);

        [Put("/api/v2/{category}/{collection}/{key}")]
        Task<object> Update(string category, string collection, string key, [Body] MetadataModelContract model);

        [Patch("/api/v2/{category}/{collection}")]
        Task BulkUpdate(string category, string collection, 
            [Body, Required] Dictionary<string, MetadataModelContract> model);

        [Delete("/api/v2/{category}/{collection}/{key}")]
        Task<object> Delete(string category, string collection, string key);

        [Delete("/api/v2/{category}/{collection}")]
        Task<object> BulkDelete(string category, string collection, [Body] HashSet<string> keys);

        [Get("/api/v2")]
        Task<List<string>> GetCategories();

        [Get("/api/v2/{category}")]
        Task<List<string>> GetCollections(string category);

        [Get("/api/v2/{category}/{collection}")]
        Task<Dictionary<string, string>> GetKeysWithData(string category, string collection, [Query] string keyword);

        [Post("/api/v2/{category}/{collection}/find")]
        Task<IDictionary<string, string>> FindByKeys(string category, string collection,
            [Body, Required] HashSet<string> keys, [Query] string keyword);

        [Get("/api/v2/{category}/{collection}/{key}")]
        Task<MetadataModelContract> Get(string category, string collection, string key);
    }
}