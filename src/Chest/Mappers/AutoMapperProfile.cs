using AutoMapper;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Data.Entities;
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
        }
    }
}