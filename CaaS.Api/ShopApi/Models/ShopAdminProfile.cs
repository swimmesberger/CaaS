using AutoMapper;
using CaaS.Core.ShopAggregate;

namespace CaaS.Api.ShopApi.Models; 

public class ShopAdminProfile : Profile {
    public ShopAdminProfile() {
        CreateMap<ShopAdmin, ShopAdminDto>().ReverseMap();
    }
}