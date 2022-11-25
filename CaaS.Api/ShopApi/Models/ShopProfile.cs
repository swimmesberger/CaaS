using AutoMapper;
using CaaS.Core.ShopAggregate;

namespace CaaS.Api.ShopApi.Models; 

public class ShopProfile : Profile {

    public ShopProfile() {
        CreateMap<Shop, ShopDto>();
        
        CreateMap<ShopForCreationDto, Shop>().ForMember(s => s.ShopAdmin,
            opt => opt.MapFrom(src => new ShopAdmin() { Id = src.ShopAdminId }));
    }
    
}