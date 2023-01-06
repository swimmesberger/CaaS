using AutoMapper;
using CaaS.Core.ShopAggregate;

namespace CaaS.Api.ShopApi.Models; 

public class ShopProfile : Profile {

    public ShopProfile() {
        CreateMap<Shop, ShopDto>();
        CreateMap<Shop, ShopMinimalDto>();

        CreateMap<ShopForCreationDto, Shop>();
        CreateMap<ShopAdminForCreationDto, ShopAdmin>();
        
        CreateMap<ShopForUpdateDto, Shop>().ForMember(c => c.Id, 
            opt => opt.MapFrom(c => Guid.Empty));
        CreateMap<ShopAdminForUpdateDto, ShopAdmin>().ForMember(c => c.Id, 
            opt => opt.MapFrom(c => c.Id ?? Guid.Empty));
    }
    
}