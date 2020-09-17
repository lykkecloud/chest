using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Data.Entities;
using Chest.Extensions;
using Chest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chest.Controllers.v2
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/localized-values")]
    [ApiController]
    [Authorize]
    public class LocalizedValuesController : ControllerBase, ILocalizedValuesApi
    {
        private readonly ILocalizedValuesService localizedValuesService;
        private readonly IMapper mapper;

        public LocalizedValuesController(ILocalizedValuesService localizedValuesService, IMapper mapper)
        {
            this.localizedValuesService = localizedValuesService;
            this.mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Add(
            [FromBody] AddLocalizedValueRequest value)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var result =
                await localizedValuesService.AddAsync(mapper.Map<LocalizedValue>(value), value.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpPut("{locale}/{key}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Update(string locale, string key,
            [FromBody] UpdateLocalizedValueRequest value)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();
            var correlationId = this.TryGetCorrelationId();

            var model = mapper.Map<LocalizedValue>(value);
            model.Locale = locale;
            model.Key = key;

            var result = await localizedValuesService.UpdateAsync(model, value.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpDelete("{locale}/{key}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Delete(string locale, string key,
            [FromBody] DeleteLocalizedValueRequest request)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();
            var correlationId = this.TryGetCorrelationId();

            var result = await localizedValuesService.DeleteAsync(locale, key, request.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet("{locale}/{key}")]
        [ProducesResponseType(typeof(GetLocalizedValueResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalizedValueResponse> Get(string locale, string key)
        {
            var response = new GetLocalizedValueResponse();
            var result = await localizedValuesService.GetAsync(locale, key);

            if (result.IsSuccess)
            {
                response.LocalizedValue =  mapper.Map<LocalizedValueContract>(result.Value);
            }
            else
            {
                response.ErrorCode = mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet("{locale}")]
        [ProducesResponseType(typeof(GetLocalizedValuesByLocaleResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalizedValuesByLocaleResponse> Get(string locale)
        {
            var localizedValues = await localizedValuesService.GetByLocaleAsync(locale);

            return new GetLocalizedValuesByLocaleResponse()
            {
                LocalizedValues = localizedValues
                    .ToDictionary(localizedValue => localizedValue.Key, localizedValue => localizedValue.Value),
            };
        }
    }
}