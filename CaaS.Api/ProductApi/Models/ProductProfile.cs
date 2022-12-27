using AutoMapper;
using CaaS.Core.ProductAggregate;

namespace CaaS.Api.ProductApi.Models; 

// ReSharper disable once UnusedType.Global
public class ProductProfile : Profile {
    public ProductProfile() {
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductMinimalDto>();

        CreateMap<ProductForCreationDto, Product>();
        CreateMap<ProductForUpdateDto, Product>();
    }
}