using AutoMapper;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Api.DiscountApi.Models; 

public class DiscountProfile : Profile {
    public DiscountProfile() {
        CreateMap<Discount, DiscountDto>();
        
        CreateMap<DiscountSetting, DiscountSettingRaw>();
        CreateMap<DiscountSettingForCreationDto, DiscountSettingRaw>();
        CreateMap<DiscountSettingForUpdateDto, DiscountSettingRaw>();
    }
}