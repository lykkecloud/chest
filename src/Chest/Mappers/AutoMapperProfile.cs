using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses;
using Chest.Core;
using Chest.Data.Entities;
using Chest.Models.v2;
using Chest.Models.v2.Audit;
using Chest.Models.v2.Locales;

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

            // locales
            CreateMap<Locale, LocaleContract>();
            CreateMap<LocalesErrorCodes, LocalesErrorCodesContract>();
            CreateMap<UpsertLocaleRequest, Locale>();

            // errors
            CreateMap<List<ValidationError>, IReadOnlyDictionary<string, string>>()
                .ConstructUsing(error => error
                    .ToDictionary(x => x.Key, x => x.Message));
            
            // audit
            CreateMap<GetAuditLogsRequest, AuditLogsFilterDto>();
            CreateMap<IAuditModel, AuditContract>();

        }
    }
}