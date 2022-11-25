using AutoMapper;
using CaaS.Core.CartAggregate;

namespace CaaS.Api.CartApi.Models; 

public class CartProfile : Profile {
    public CartProfile() {
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>();
    }
}