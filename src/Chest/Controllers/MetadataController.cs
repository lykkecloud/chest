// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Controllers
{
    using System.Collections.Generic;
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

        [HttpDelete("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Remove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string category, string collection, string key)
        {
            await this.service.Delete(category, collection, key);

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
