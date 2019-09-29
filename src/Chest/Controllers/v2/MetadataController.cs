// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authorization;

#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Controllers.v2
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Exceptions;
    using Chest.Models.v2;
    using Chest.Services;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/")]
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
            await this.service.Add(category, collection, key, model.Data, model.Keywords);

            return this.Created(this.Request.GetRelativeUrl($"api/v2/{category}/{collection}/{key}"), model);
        }

        [HttpPost("{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkAdd")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> BulkCreate(string category, string collection, [FromBody]Dictionary<string, MetadataModel> model)
        {
            await this.service.BulkAdd(category, collection, model.ToDictionary(x => x.Key, x => (x.Value.Data, x.Value.Keywords)));

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
            await this.service.Update(category, collection, key, model.Data, model.Keywords);

            return this.Ok(new { Message = "Update successfully" });
        }

        [HttpPatch("{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> BulkUpdate(string category, string collection, [FromBody, Required]Dictionary<string, MetadataModel> model)
        {
            await this.service.BulkUpdate(category, collection, model.ToDictionary(x => x.Key, x => (x.Value.Data, x.Value.Keywords)));

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
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Dictionary<string, string>))]
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

        // NOTE: This is POST because passing around massive strings in a query parameter might
        // hit some URL length limitation along the way
        [HttpPost("{category}/{collection}/find")]
        [SwaggerOperation("Metadata_FindByKeys")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IDictionary<string, string>))]
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

            return this.Ok(data);
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

            return this.Ok(new MetadataModel { Data = data, Keywords = keywords });
        }
    }
}
