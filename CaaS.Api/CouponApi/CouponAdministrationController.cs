using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CouponApi.Models;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
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
    private readonly IStatisticsService _statisticsService;
    
    public CouponAdministrationController(ICouponService couponService, IMapper mapper, IStatisticsService statisticsService) {
        _couponService = couponService;
        _mapper = mapper;
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IEnumerable<CouponDto>> Get([FromQuery] Guid? cartId, [FromQuery] Guid? orderId, [FromQuery] Guid? customerId, 
        [FromQuery] string? code, CancellationToken cancellationToken = default) {
        var result =  await _couponService.GetByAsync(new CouponQuery() {
            CartId = cartId, 
            OrderId = orderId,
            CustomerId = customerId,
            Code = code
        }, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<CouponDto>>(result.Items);
    }

    [HttpGet("{couponId:guid}")]
    public async Task<CouponDto?> GetById(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetByIdAsync(couponId, cancellationToken);
        return _mapper.Map<CouponDto?>(coupon);
    }
    
    [HttpGet("code/{couponCode}")]
    public async Task<CouponDto?> GetByCode(string couponCode, CancellationToken cancellationToken = default) {
        var coupon = await _couponService.GetByCodeAsync(couponCode, cancellationToken);
        return _mapper.Map<CouponDto?>(coupon);
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
        updatedCoupon = updatedCoupon with { Id = couponId };
        var result = await _couponService.UpdateAsync(updatedCoupon, cancellationToken);
        return _mapper.Map<CouponDto>(result);
    }
    
    [HttpDelete("{couponId:guid}")]
    public async Task<ActionResult> DeleteCoupon(Guid couponId, CancellationToken cancellationToken = default) {
        await _couponService.DeleteAsync(couponId, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("CouponStatisticsOverall")]
    public async Task<CouponStatisticsResult> CouponStatisticsOverall([FromQuery] [Required] DateTimeOffset from, [FromQuery] DateTimeOffset? until = null, 
        CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until != null && until.Value.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.CouponStatisticsOverall(from, until, cancellationToken: cancellationToken);
    }
}