using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Data.Entities;
using Chest.Exceptions;
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

            try
            {
                await localizedValuesService.AddAsync(mapper.Map<LocalizedValue>(value));
            }
            catch (LocalizedValueAlreadyExistsException)
            {
                response.ErrorCode = LocalizedValuesErrorCodesContract.LocalizedValueAlreadyExists;
            }

            return response;
        }

        [HttpPut("{locale}/{key}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Update(string locale, string key,
            [FromBody] UpdateLocalizedValueRequest value)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();

            var model = mapper.Map<LocalizedValue>(value);
            model.Locale = locale;
            model.Key = key;

            try
            {
                await localizedValuesService.UpdateAsync(model);
            }
            catch (LocalizedValueNotFoundException)
            {
                response.ErrorCode = LocalizedValuesErrorCodesContract.LocalizedValueDoesNotExist;
            }

            return response;
        }

        [HttpDelete("{locale}/{key}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<LocalizedValuesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<LocalizedValuesErrorCodesContract>> Delete(string locale, string key, [FromBody] DeleteLocalizedValueRequest request)
        {
            var response = new ErrorCodeResponse<LocalizedValuesErrorCodesContract>();
            try
            {
                await localizedValuesService.DeleteAsync(locale, key);
            }
            catch (LocalizedValueNotFoundException)
            {
                response.ErrorCode = LocalizedValuesErrorCodesContract.LocalizedValueDoesNotExist;
            }

            return response;
        }

        [HttpGet("{locale}/{key}")]
        [ProducesResponseType(typeof(GetLocalizedValueResponse), (int) HttpStatusCode.OK)]
        public async Task<GetLocalizedValueResponse> Get(string locale, string key)
        {
            var response = new GetLocalizedValueResponse();
            var localizedValue = await localizedValuesService.GetAsync(locale, key);

            if (localizedValue == null)
            {
                response.ErrorCode = LocalizedValuesErrorCodesContract.LocalizedValueDoesNotExist;
                return response;
            }

            response.LocalizedValue = mapper.Map<LocalizedValueContract>(localizedValue);

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