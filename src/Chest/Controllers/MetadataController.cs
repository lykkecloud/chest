// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Models;
    using Chest.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService service;

        public MetadataController(IMetadataService service)
        {
            this.service = service;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var dict = await this.service.GetAsync(key);

            if (dict == null)
            {
                return this.NotFound(new { Message = $"No data found for key: {key}" });
            }

            return this.Ok(
                new MetadaModel
                {
                    Key = key,
                    Data = dict
                });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]MetadaModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(new { Message = string.Join(", ", this.ModelState.Values.Select(m => string.Join(", ", m.Errors.Select(e => e.ErrorMessage)))) });
            }

            var dict = await this.service.GetAsync(model.Key);

            if (dict != null)
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = $"Data already exists for key: {model.Key}" });
            }

            await this.service.SaveAsync(model.Key, model.Data);

            return this.Created(this.Request.GetRelativeUrl($"api/metadata/{model.Key}"), model);
        }
    }
}
