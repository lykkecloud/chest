using AutoMapper;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Data.Entities;

namespace Chest.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LocalizedValue, LocalizedValueContract>();

            CreateMap<AddLocalizedValueRequest, LocalizedValue>();
            CreateMap<UpdateLocalizedValueRequest, LocalizedValue>()
                .ForMember(x => x.Locale, opt => opt.Ignore())
                .ForMember(x => x.Key, opt => opt.Ignore());
        }
    }
}