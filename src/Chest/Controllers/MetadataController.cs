// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Exceptions;
    using Chest.Models;
    using Chest.Services;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class MetadataController : ControllerBase
    {
        private readonly IDataService service;

        public MetadataController(IDataService service)
        {
            this.service = service;
        }

        [HttpPost("api/{category}/{collection}/{key}")]
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
                await this.service.Add(category, collection, key, model.Data, model.Keywords);
            }
            catch (DuplicateKeyException)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = $"Data already exists for category: {category} collection: {collection} key: {key}" });
            }

            return this.Created(this.Request.GetRelativeUrl($"api/{category}/{collection}/{key}"), model);
        }

        [HttpPost("api/{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkAdd")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> BulkCreate(string category, string collection, [FromBody]Dictionary<string, MetadataModel> model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                await this.service.BulkAdd(category, collection, model.ToDictionary(x => x.Key, x => (x.Value.Data, x.Value.Keywords)));
            }
            catch (DuplicateKeyException e)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { e.Message });
            }

            // Opted for 200 OK instead of 201 Created since you can't specify multiple items
            return this.Ok();
        }

        [HttpPut("api/{category}/{collection}/{key}")]
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
                await this.service.Update(category, collection, key, model.Data, model.Keywords);
            }
            catch (NotFoundException)
            {
                return this.NotFound(new { Message = $"No record found against category: {category} collection: {collection} key: {key}" });
            }

            return this.Ok(new { Message = "Update successfully" });
        }

        [HttpPatch("api/{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> BulkUpdate(string category, string collection, [FromBody, Required]Dictionary<string, MetadataModel> model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                await this.service.BulkUpdate(category, collection, model.ToDictionary(x => x.Key, x => (x.Value.Data, x.Value.Keywords)));
            }
            catch (NotFoundException e)
            {
                return this.NotFound(new { e.Message });
            }

            return this.Ok();
        }

        [HttpDelete("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Remove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string category, string collection, string key)
        {
            await this.service.Delete(category, collection, key);

            return this.Ok(new { Message = "Deleted successfully" });
        }

        [HttpDelete("api/{category}/{collection}")]
        [SwaggerOperation("Metadata_BulkRemove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> BulkDelete(string category, string collection, [FromBody]HashSet<string> keys)
        {
            await this.service.BulkDelete(category, collection, keys);

            return this.Ok(new { Message = "Deleted successfully" });
        }

        [HttpGet("api")]
        [SwaggerOperation("Metadata_GetCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(List<string>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await this.service.GetCategories();

            return this.Ok(categories);
        }

        [HttpGet("api/{category}")]
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

        [HttpGet("api/{category}/{collection}")]
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

        // NOTE: This is POST because passing around massive strings in a query parameter might
        // hit some URL length limitation along the way
        [HttpPost("api/{category}/{collection}/find")]
        [SwaggerOperation("Metadata_FindByKeys")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IDictionary<string, IDictionary<string, string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByKeys(
            string category,
            string collection,
            [FromBody, Required]HashSet<string> keys,
            [FromQuery]string keyword)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var data = await this.service.FindByKeys(category, collection, keys, keyword);

            var missingKeys = keys.Where(x => !data.ContainsKey(x)).ToArray();

            if (missingKeys.Length > 0)
            {
                return this.NotFound(new { Message = $"No data found for category: {category} collection: {collection} and keys: {string.Join(", ", missingKeys)}" });
            }

            return this.Ok(data);
        }

        [HttpGet("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(MetadataModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string category, string collection, string key)
        {
            var data = await this.service.Get(category, collection, key);

            if (data.keyValuePairs == null)
            {
                return this.NotFound(new { Message = $"No data found for category: {category} collection: {collection} and key: {key}" });
            }

            return this.Ok(new MetadataModel { Data = data.keyValuePairs, Keywords = data.keywords });
        }
    }
}
