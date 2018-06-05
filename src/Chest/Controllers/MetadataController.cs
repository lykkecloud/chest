// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Exceptions;
    using Chest.Models;
    using Chest.Services;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService service;

        public MetadataController(IMetadataService service)
        {
            this.service = service;
        }

        [HttpGet("{key}")]
        [SwaggerOperation("Metadata_Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MetadataModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this.BadRequest(new { Message = "Cannot get data without specifying a key" });
            }

            Dictionary<string, string> keyValueData = null;

            try
            {
                keyValueData = await this.service.Get(key);

                if (keyValueData == null)
                {
                    return this.NotFound(new { Message = $"No data found for key: {key}" });
                }
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while fetching data | Error: {exp.Message}" });
            }

            return this.Ok(
                new MetadataModel
                {
                    Key = key,
                    Data = keyValueData
                });
        }

        [HttpPost]
        [SwaggerOperation("Metadata_Add")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Post([FromBody]MetadataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(new { Message = string.Join(", ", this.ModelState.Values.Select(m => string.Join(", ", m.Errors.Select(e => e.ErrorMessage)))) });
            }

            try
            {
                await this.service.Save(model.Key, model.Data);
            }
            catch (DuplicateKeyException)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = $"Data already exists for key: {model.Key}" });
            }
            catch (Exception exp)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"An unknown error occured while saving data | Error: {exp.Message}" });
            }

            return this.Created(this.Request.GetRelativeUrl($"api/metadata/{model.Key}"), model);
        }
    }
}
