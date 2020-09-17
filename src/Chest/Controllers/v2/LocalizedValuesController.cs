using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Core;
using Chest.Data.Entities;
using Chest.Extensions;
using Chest.Models.v2.Locales;
using Chest.Models.v2.LocalizedValues;
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
        private readonly ILocalizedValuesService _localizedValuesService;
        private readonly IMapper _mapper;

        public LocalizedValuesController(ILocalizedValuesService localizedValuesService, IMapper mapper)
        {
            _localizedValuesService = localizedValuesService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Add(
            [FromBody] AddLocalizedValueRequest value)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var result =
                await _localizedValuesService.AddAsync(_mapper.Map<LocalizedValue>(value), value.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = _mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
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

            var model = _mapper.Map<LocalizedValue>(value);
            model.Locale = locale;
            model.Key = key;

            var result = await _localizedValuesService.UpdateAsync(model, value.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = _mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
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

            var result = await _localizedValuesService.DeleteAsync(locale, key, request.UserName, correlationId);
            if (result.IsFailed)
            {
                response.ErrorCode = _mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet("{locale}/{key}")]
        [ProducesResponseType(typeof(GetLocalizedValueResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalizedValueResponse> Get(string locale, string key)
        {
            var response = new GetLocalizedValueResponse();
            var result = await _localizedValuesService.GetAsync(locale, key);

            if (result.IsSuccess)
            {
                response.LocalizedValue = _mapper.Map<LocalizedValueContract>(result.Value);
            }
            else
            {
                response.ErrorCode = _mapper.Map<LocalizedValuesErrorCodesContract>(result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet("{locale}")]
        [ProducesResponseType(typeof(GetLocalizedValuesByLocaleResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalizedValuesByLocaleResponse> Get(string locale)
        {
            var localizedValues = await _localizedValuesService.GetByLocaleAsync(locale);

            return new GetLocalizedValuesByLocaleResponse()
            {
                LocalizedValues = localizedValues
                    .ToDictionary(localizedValue => localizedValue.Key, localizedValue => localizedValue.Value),
            };
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetAllLocalizedValuesResponse), (int) HttpStatusCode.OK)]
        public async Task<GetAllLocalizedValuesResponse> Get(int skip = default, int take = 0)
        {
            var result = await _localizedValuesService.GetAllAsync(skip, take);
            var response = _mapper.Map<GetAllLocalizedValuesResponse>(result);

            return response;
        }

        [HttpPut]
        [ProducesResponseType(typeof(UpsertLocalizedValuesByKeyErrorCodeResponse), (int) HttpStatusCode.OK)]
        public async Task<UpsertLocalizedValuesByKeyErrorCodeResponse> UpsertByKey(
            UpsertLocalizedValuesByKeyRequest request)
        {
            var response = new UpsertLocalizedValuesByKeyErrorCodeResponse();
            var correlationId = this.TryGetCorrelationId();
            
            var result = await _localizedValuesService.UpsertByKey(request.Key,
                _mapper.Map<Dictionary<string, string>>(request.Localizations), request.UserName, correlationId);
            
            if (result.IsFailed)
            {
                if (result is ErrorResult<LocalizedValuesErrorCodes> r)
                {
                    response.Errors = _mapper.Map<IReadOnlyList<ValidationErrorContract>>(r.ToValidationErrors());
                }
                else
                {
                    response.Errors =
                        _mapper.Map<IReadOnlyList<ValidationErrorContract>>(new ErrorResult<LocalizedValuesErrorCodes>(result)
                            .ToValidationErrors());
                }
            }

            return response;
        }
    }
}