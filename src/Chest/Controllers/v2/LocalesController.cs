using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Data.Entities;
using Chest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chest.Controllers.v2
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/locales")]
    [ApiController]
    [Authorize]
    public class LocalesController : ControllerBase, ILocalesApi
    {
        private readonly ILocalesService _localesService;
        private readonly IMapper _mapper;

        public LocalesController(ILocalesService localesService, IMapper mapper)
        {
            _localesService = localesService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetLocalesResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalesResponse> GetAllAsync()
        {
            var locales = await _localesService.GetAllAsync();

            var response = new GetLocalesResponse {Locales = _mapper.Map<List<LocaleContract>>(locales.Value)};

            return response;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UpsertLocaleErrorCodeResponse), (int) HttpStatusCode.OK)]
        public async Task<UpsertLocaleErrorCodeResponse> UpsertAsync([FromBody] UpsertLocaleRequest request)
        {
            var response = new UpsertLocaleErrorCodeResponse();

            var result = await _localesService.UpsertAsync(_mapper.Map<Locale>(request));

            if (result.IsFailed)
            {
                // todo: keys
                response.ErrorCode = _mapper.Map<LocalesErrorCodesContract>(result.Error);
            }

            return response;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalesErrorCodesContract>> DeleteAsync([FromRoute] string id, [FromBody] DeleteLocaleRequest request)
        {
            var response = new ErrorCodeResponse<LocalesErrorCodesContract>();

            var result = await _localesService.DeleteAsync(id);

            if (result.IsFailed)
            {
                response.ErrorCode = _mapper.Map<LocalesErrorCodesContract>(result.Error);
            }

            return response;
        }
    }
}