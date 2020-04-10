// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Models.v1;
    using Chest.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Annotations;

    [Obsolete("MetadataController is obsolete, please use v2/MetadataController instead.")]
    [ApiVersion("1")]
    [Route("api")]
    [ApiController]
    [Authorize]
    public class MetadataController : ControllerBase
    {
        private readonly IDataService service;

        public MetadataController(IDataService service)
        {
            this.service = service;
        }

        [HttpPost("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Add")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Create(string category, string collection, string key, [FromBody]MetadataModel model)
        {
            var serializedData = JsonConvert.SerializeObject(model.Data);
            var serializedKeywords = model.Keywords == null ? string.Empty : JsonConvert.SerializeObject(model.Keywords);
            await this.service.Add(category, collection, key, serializedData, serializedKeywords);

            return this.Created(this.Request.GetRelativeUrl($"api/{category}/{collection}/{key}"), model);
        }

        [HttpPost("{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkAdd")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> BulkCreate(string category, string collection, [FromBody]Dictionary<string, MetadataModel> model)
        {
            var serializedModel = model.ToDictionary(x => x.Key, x =>
            {
                var serializedData = JsonConvert.SerializeObject(x.Value.Data);
                var serializedKeywords = x.Value.Keywords == null ? string.Empty : JsonConvert.SerializeObject(x.Value.Keywords);
                return (serializedData, serializedKeywords);
            });
            await this.service.BulkAdd(category, collection, serializedModel);

            // Opted for 200 OK instead of 201 Created since you can't specify multiple items
            return this.Ok();
        }

        [HttpPut("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Update")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(string category, string collection, string key, [FromBody]MetadataModel model)
        {
            var serializedData = JsonConvert.SerializeObject(model.Data);
            var serializedKeywords = model.Keywords == null ? string.Empty : JsonConvert.SerializeObject(model.Keywords);
            await this.service.Upsert(category, collection, key, serializedData, serializedKeywords);

            return this.Ok(new { Message = "Update successfully" });
        }

        [HttpPatch("{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> BulkUpdate(string category, string collection, [FromBody, Required]Dictionary<string, MetadataModel> model)
        {
            var serializedModel = model.ToDictionary(x => x.Key, x =>
            {
                var serializedData = JsonConvert.SerializeObject(x.Value.Data);
                var serializedKeywords = x.Value.Keywords == null ? string.Empty : JsonConvert.SerializeObject(x.Value.Keywords);
                return (serializedData, serializedKeywords);
            });
            await this.service.BulkUpsert(category, collection, serializedModel);

            return this.Ok();
        }

        [HttpDelete("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Remove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string category, string collection, string key)
        {
            await this.service.Delete(category, collection, key);

            return this.Ok(new { Message = "Deleted successfully" });
        }

        [HttpDelete("{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkRemove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> BulkDelete(string category, string collection, [FromBody]HashSet<string> keys)
        {
            await this.service.BulkDelete(category, collection, keys);

            return this.Ok(new { Message = "Deleted successfully" });
        }

        [HttpGet("")]
        [SwaggerOperation("Metadata_GetCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<string>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await this.service.GetCategories();

            return this.Ok(categories);
        }

        [HttpGet("{category}")]
        [SwaggerOperation("Metadata_GetCollections")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCollections(string category)
        {
            var collections = await this.service.GetCollections(category);

            if (!collections.Any())
            {
                return this.NotFound(new { Message = $"Category: {category} doesn't exist" });
            }

            return this.Ok(collections);
        }

        [HttpGet("{category}/{collection}")]
        [SwaggerOperation("Metadata_GetKeysWithData")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(Dictionary<string, Dictionary<string, string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetKeysWithData(string category, string collection, [FromQuery]string keyword)
        {
            var keyValueData = await this.service.GetKeyValues(category, collection, keyword);

            if (!keyValueData.Any())
            {
                return this.NotFound(new { Message = $"No record found for Category: {category} Collection: {collection} filtered by Keyword: {keyword}" });
            }

            var deserializedKeyValueData = keyValueData.ToDictionary(keyValue => keyValue.Key, keyValue => JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValue.Value));

            return this.Ok(deserializedKeyValueData);
        }

        // NOTE: This is POST because passing around massive strings in a query parameter might
        // hit some URL length limitation along the way
        [HttpPost("{category}/{collection}/find")]
        [SwaggerOperation("Metadata_FindByKeys")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(IDictionary<string, IDictionary<string, string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByKeys(
            string category,
            string collection,
            [FromBody, Required]HashSet<string> keys,
            [FromQuery]string keyword)
        {
            var data = await this.service.FindByKeys(category, collection, keys, keyword);

            var missingKeys = keys.Where(x => !data.ContainsKey(x)).ToArray();
            if (missingKeys.Length > 0)
            {
                return this.NotFound(new { Message = $"No data found for category: {category} collection: {collection} and keys: {string.Join(", ", missingKeys)}" });
            }

            var deserializedData = data.ToDictionary(keyValue => keyValue.Key, keyValue => JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValue.Value));
            return this.Ok(deserializedData);
        }

        [HttpGet("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(MetadataModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string category, string collection, string key)
        {
            var (data, keywords) = await this.service.Get(category, collection, key);

            if (string.IsNullOrWhiteSpace(data))
            {
                return this.NotFound(new { Message = $"No data found for category: {category} collection: {collection} and key: {key}" });
            }

            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var deserializedKeywords = string.IsNullOrWhiteSpace(keywords) ? default(List<string>) : JsonConvert.DeserializeObject<List<string>>(keywords);

            return this.Ok(new MetadataModel { Data = deserializedData, Keywords = deserializedKeywords });
        }
    }
}
