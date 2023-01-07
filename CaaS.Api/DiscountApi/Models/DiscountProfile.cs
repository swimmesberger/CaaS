using AutoMapper;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Api.DiscountApi.Models; 

public class DiscountProfile : Profile {
    public DiscountProfile() {
        CreateMap<Discount, DiscountDto>();
        
        CreateMap<DiscountSetting, DiscountSettingRaw>();
        CreateMap<DiscountSettingForCreationDto, DiscountSettingRaw>();
        CreateMap<DiscountMetadataSettingRawForCreationDto, DiscountMetadataSettingRaw>().ForMember(c => c.Id, 
            opt => opt
                .MapFrom(c => c.Id ?? Guid.Empty));
        CreateMap<DiscountSettingForUpdateDto, DiscountSettingRaw>();
    }
}