using AutoMapper;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.ProductAggregate;

namespace CaaS.Api.CartApi.Models; 

public class CartProfile : Profile {
    public CartProfile() {
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>();
        
        CreateMap<CartForUpdateDto, Cart>();
        CreateMap<CartForUpdateItemDto, CartItem>();
        CreateMap<CartForUpdateCouponDto, Coupon>().ForMember(c => c.Id, 
            opt => opt.MapFrom(c => Guid.Empty));
        CreateMap<CartForUpdateCartItemProductDto, Product>();
    }
}