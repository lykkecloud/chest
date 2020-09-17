using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Core;
using Chest.Extensions;
using Chest.Data.Entities;
using Chest.Models.v2.Locales;
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

            var correlationId = this.TryGetCorrelationId();
            var result =
                await _localesService.UpsertAsync(_mapper.Map<Locale>(request), request.UserName, correlationId);

            // todo: move validations to the base class to avoid pattern matching
            if (result.IsFailed)
            {
                if (result is ErrorResult<LocalesErrorCodes> r)
                {
                    response.Errors = _mapper.Map<IReadOnlyList<ValidationErrorContract>>(r.ToValidationErrors());
                }
                else
                {
                    response.Errors =
                        _mapper.Map<IReadOnlyList<ValidationErrorContract>>(new ErrorResult<LocalesErrorCodes>(result)
                            .ToValidationErrors());
                }
            }

            return response;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorsResponse), (int) HttpStatusCode.OK)]
        public async Task<ErrorsResponse> DeleteAsync([FromRoute] string id,
            [FromBody] DeleteLocaleRequest request)
        {
            var response = new ErrorsResponse();

            var correlationId = this.TryGetCorrelationId();
            var result = await _localesService.DeleteAsync(id, request.UserName, correlationId);

            if (result.IsFailed)
            {
                // todo: move validations to the base class to avoid cast from base class
                var r = new ErrorResult<LocalesErrorCodes>(result);
                response.Errors = _mapper.Map<IReadOnlyList<ValidationErrorContract>>(r.ToValidationErrors());
            }

            return response;
        }
    }
}