// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Controllers.v1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Exceptions;
    using Chest.Models.v1;
    using Chest.Services;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Obsolete("MetadataController is obsolete, please use v2/MetadataController instead.")]
    [ApiVersion("1")]
    [Route("api")]
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
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                var serializedData = JsonConvert.SerializeObject(model.Data);
                var serializedKeywords = JsonConvert.SerializeObject(model.Keywords);
                await this.service.Add(category, collection, key, serializedData, serializedKeywords);
            }
            catch (DuplicateKeyException)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = $"Data already exists for category: {category} collection: {collection} key: {key}" });
            }

            return this.Created(this.Request.GetRelativeUrl($"api/{category}/{collection}/{key}"), model);
        }

        [HttpPut("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Update")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(string category, string collection, string key, [FromBody]MetadataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                var serializedData = JsonConvert.SerializeObject(model.Data);
                var serializedKeywords = JsonConvert.SerializeObject(model.Keywords);
                await this.service.Update(category, collection, key, serializedData, serializedKeywords);
            }
            catch (NotFoundException)
            {
                return this.NotFound(new { Message = $"No record found against category: {category} collection: {collection} key: {key}" });
            }

            return this.Ok(new { Message = "Update successfully" });
        }

        [HttpDelete("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Remove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string category, string collection, string key)
        {
            await this.service.Delete(category, collection, key);

            return this.Ok(new { Message = "Deleted successfully" });
        }

        [HttpGet("")]
        [SwaggerOperation("Metadata_GetCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(List<string>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await this.service.GetCategories();

            return this.Ok(categories);
        }

        [HttpGet("{category}")]
        [SwaggerOperation("Metadata_GetCollections")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(List<string>))]
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
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Dictionary<string, Dictionary<string, string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetKeysWithData(string category, string collection, [FromQuery]string keyword)
        {
            var keyValueData = await this.service.GetKeyValues(category, collection, keyword);

            if (!keyValueData.Any())
            {
                return this.NotFound(new { Message = $"No record found for Category: {category} Collection: {collection} filtered by Keyword: {keyword}" });
            }

            return this.Ok(keyValueData);
        }

        [HttpGet("{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(MetadataModel))]
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
