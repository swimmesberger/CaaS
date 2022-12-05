using AutoMapper;
using CaaS.Core.CouponAggregate;

namespace CaaS.Api.CouponApi.Models; 

public class CouponProfile : Profile {
    public CouponProfile() {
        CreateMap<Coupon, CouponDto>();
        CreateMap<CouponForCreationDto, Coupon>();
        CreateMap<CouponForUpdateDto, Coupon>();
    }
}