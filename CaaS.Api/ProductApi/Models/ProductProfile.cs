using AutoMapper;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;

namespace CaaS.Api.ProductApi.Models; 

// ReSharper disable once UnusedType.Global
public class ProductProfile : Profile {
    public ProductProfile() {
        CreateMap<Product, ProductDto>().ForMember(p => p.ShopId,
            opt => opt.MapFrom(src => src.Shop.Id));

        CreateMap<ProductForCreationDto, Product>();
    }
}