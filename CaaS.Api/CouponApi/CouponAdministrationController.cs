using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CouponApi.Models;
using CaaS.Core.CouponAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CouponApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class CouponAdministrationController : ControllerBase {
    private readonly ICouponService _couponService;
    private readonly IMapper _mapper;
    
    public CouponAdministrationController(ICouponService couponService, IMapper mapper) {
        _couponService = couponService;
        _mapper = mapper;
    }

    [HttpGet("getById/{couponId:guid}")]
    public async Task<CouponDto> GetCouponById(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetCouponById(couponId, cancellationToken);
        return _mapper.Map<CouponDto>(coupon);
    }
    
    [HttpGet("getByCartId/{cartId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetCouponsByCartId(cartId, cancellationToken);
        return _mapper.Map<IEnumerable<CouponDto>>(coupon);
    }
    
    [HttpGet("getByOrderId/{orderId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetCouponsByOrderId(orderId, cancellationToken);
        return _mapper.Map<IEnumerable<CouponDto>>(coupon);
    }
    
    [HttpGet("getByCustomerId/{customerId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetCouponsByCustomerId(customerId, cancellationToken);
        return _mapper.Map<IEnumerable<CouponDto>>(coupon);
    }

    [HttpPut("{couponId:guid}")]
    public async Task<CouponDto> UpdateCoupon(Guid couponId, CouponForUpdateDto couponDto, CancellationToken cancellationToken = default) {
        var updatedCoupon = _mapper.Map<Coupon>(couponDto);
        var result = await _couponService.UpdateCoupon(couponId, updatedCoupon, cancellationToken);
        return _mapper.Map<CouponDto>(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> AddCoupon(CouponForCreationDto couponDto, CancellationToken cancellationToken = default) {
        var coupon = _mapper.Map<Coupon>(couponDto);
        var result = await _couponService.AddCoupon(coupon, cancellationToken);
        
        return CreatedAtAction(
            actionName: nameof(AddCoupon),
            routeValues: new { couponId = result.Id },
            value: _mapper.Map<CouponDto>(result));
    }

    [HttpDelete("{couponId:guid}")]
    public async Task<ActionResult> DeleteCoupon(Guid couponId, CancellationToken cancellationToken = default) {
        await _couponService.DeleteCoupon(couponId, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("setValue/{couponId:guid}")]
    public async Task<CouponDto> SetValueOfCoupon(Guid couponId, [FromQuery][Required] decimal value,
        CancellationToken cancellationToken = default) {
        var result = await _couponService.SetValueOfCoupon(couponId, value, cancellationToken);
        return _mapper.Map<CouponDto>(result);
    }
}