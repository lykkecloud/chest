using AutoMapper;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Data.Entities;
using Chest.Models.v2;
using Chest.Models.v2.Audit;
using Chest.Models.v2.Locales;
using Chest.Models.v2.LocalizedValues;
using ValidationError = Chest.Core.ValidationError;

namespace Chest.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // localized values
            CreateMap<LocalizedValue, LocalizedValueContract>();
            CreateMap<AddLocalizedValueRequest, LocalizedValue>();
            CreateMap<UpdateLocalizedValueRequest, LocalizedValue>()
                .ForMember(x => x.Locale, opt => opt.Ignore())
                .ForMember(x => x.Key, opt => opt.Ignore());

            CreateMap<LocalizedValuesErrorCodes, LocalizedValuesErrorCodesContract>();

            // locales
            CreateMap<Locale, LocaleContract>();
            CreateMap<LocalesErrorCodes, LocalesErrorCodesContract>();
            CreateMap<UpsertLocaleRequest, Locale>();
            CreateMap<LocalizedValueByKey, LocalizedValueByKeyContract>();
            CreateMap<PaginatedResponse<LocalizedValueByKey>, GetAllLocalizedValuesResponse>();

            // errors
            CreateMap<ValidationError, ValidationErrorContract>();

            // audit
            CreateMap<GetAuditLogsRequest, AuditLogsFilterDto>();
            CreateMap<IAuditModel, AuditContract>();
        }
    }
}