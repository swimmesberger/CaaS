using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CouponApi.Models;
using CaaS.Core.CouponAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.CouponApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
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
    public async Task<CouponDto?> GetById(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetByIdAsync(couponId, cancellationToken);
        return _mapper.Map<CouponDto?>(coupon);
    }
    
    [HttpGet("getByCartId/{cartId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        var result =  await _couponService.GetByCartIdAsync(cartId, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<CouponDto>>(result.Items);
    }
    
    [HttpGet("getByOrderId/{orderId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        var result =  await _couponService.GetByOrderIdAsync(orderId, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<CouponDto>>(result.Items);
    }
    
    [HttpGet("getByCustomerId/{customerId:guid}")]
    public async Task<IEnumerable<CouponDto>> GetCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        var result =  await _couponService.GetByCustomerIdAsync(customerId, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<CouponDto>>(result.Items);
    }
    
    [HttpPost]
    public async Task<ActionResult> AddCoupon([FromBody] CouponForCreationDto couponDto, CancellationToken cancellationToken = default) {
        var coupon = _mapper.Map<Coupon>(couponDto);
        var result = await _couponService.AddAsync(coupon, cancellationToken);
        
        return CreatedAtAction(
            controllerName: "CouponAdministration",
            actionName: nameof(GetById),
            routeValues: new { couponId = result.Id },
            value: _mapper.Map<CouponDto>(result));
    }

    [HttpPut("{couponId:guid}")]
    public async Task<CouponDto> UpdateCoupon(Guid couponId, [FromBody] CouponForUpdateDto couponDto, CancellationToken cancellationToken = default) {
        var updatedCoupon = _mapper.Map<Coupon>(couponDto);
        var result = await _couponService.UpdateAsync(couponId, updatedCoupon, cancellationToken);
        return _mapper.Map<CouponDto>(result);
    }
    
    [HttpDelete("{couponId:guid}")]
    public async Task<ActionResult> DeleteCoupon(Guid couponId, CancellationToken cancellationToken = default) {
        await _couponService.DeleteAsync(couponId, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("setValue/{couponId:guid}")]
    public async Task<CouponDto> SetValueOfCoupon(Guid couponId, [FromQuery][Required] decimal value,
        CancellationToken cancellationToken = default) {
        var result = await _couponService.SetValueOfCouponAsync(couponId, value, cancellationToken);
        return _mapper.Map<CouponDto>(result);
    }
}