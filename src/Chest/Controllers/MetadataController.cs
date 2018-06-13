// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Controllers
{
    using System;
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
        private readonly IMetadataService service;

        public MetadataController(IMetadataService service)
        {
            this.service = service;
        }

        [HttpPost("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Add")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(string category, string collection, string key, [FromBody]MetadataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                await this.service.Add(category, collection, key, model.Data);
            }
            catch (DuplicateKeyException)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = $"Data already exists for category: {category} collection: {collection} key: {key}" });
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while saving data | Error: {exp.Message}" });
            }

            return this.Created(this.Request.GetRelativeUrl($"api/{category}/{collection}/{key}"), model);
        }

        [HttpPut("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Update")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(string category, string collection, string key, [FromBody]MetadataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            try
            {
                await this.service.Update(category, collection, key, model.Data);
            }
            catch (NotFoundException)
            {
                return this.NotFound(new { Message = $"No record found against category: {category} collection: {collection} key: {key}" });
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while saving data | Error: {exp.Message}" });
            }

            return this.Ok(new { Message = "Update successfully" });
        }

        [HttpDelete("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_Remove")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(string category, string collection, string key)
        {
            try
            {
                await this.service.Delete(category, collection, key);

                return this.Ok(new { Message = "Deleted successfully" });
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while deleting data | Error: {exp.Message}" });
            }
        }

        [HttpGet("api/categories")]
        [SwaggerOperation("Metadata_GetCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var collections = await this.service.GetCategories();

                return this.Ok(collections);
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while fetching data | Error: {exp.Message}" });
            }
        }

        [HttpGet("api/{category}")]
        [SwaggerOperation("Metadata_GetCollections")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCollections(string category)
        {
            try
            {
                var collections = await this.service.GetCollections(category);

                if (!collections.Any())
                {
                    return this.NotFound(new { Message = $"Category: {category} doesn't exist" });
                }

                return this.Ok(collections);
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while fetching data | Error: {exp.Message}" });
            }
        }

        [HttpGet("api/{category}/{collection}")]
        [SwaggerOperation("Metadata_GetKeysWithData")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Dictionary<string, Dictionary<string, string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetKeysWithData(string category, string collection)
        {
            try
            {
                var keyValueData = await this.service.GetKeyValues(category, collection);

                if (!keyValueData.Any())
                {
                    return this.NotFound(new { Message = $"No record found for Category: {category} and Collection: {collection}" });
                }

                return this.Ok(keyValueData);
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while fetching data | Error: {exp.Message}" });
            }
        }

        [HttpGet("api/{category}/{collection}/{key}")]
        [SwaggerOperation("Metadata_GetMetadata")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(MetadataModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMetadata(string category, string collection, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this.BadRequest(new { Message = "Cannot get data without specifying a key" });
            }

            Dictionary<string, string> keyValueData = null;

            try
            {
                keyValueData = await this.service.Get(category, collection, key);

                if (keyValueData == null)
                {
                    return this.NotFound(new { Message = $"No data found for category: {category} collection: {collection} and key: {key}" });
                }
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while fetching data | Error: {exp.Message}" });
            }

            return this.Ok(new MetadataModel { Data = keyValueData });
        }
    }
}
