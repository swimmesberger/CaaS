using AutoMapper;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.OrderApi.Models;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.OrderApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class OrderAdministrationController : ControllerBase {
    private readonly IOrderService _orderService;
    private readonly IStatisticsService _statisticsService;
    private readonly IMapper _mapper;

    public OrderAdministrationController(IOrderService orderService, IStatisticsService statisticsService, IMapper mapper) {
        _orderService = orderService;
        _statisticsService = statisticsService;
        _mapper = mapper;
    }
    
    [HttpGet("{orderId:guid}")]
    public async Task<OrderDto?> GetOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var result = await _orderService.FindByIdAsync(orderId, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
    
    [HttpGet("/AverageValueOfOrdersInTimePeriodConsideringDiscountsAndCoupons")]
    public async Task<decimal> AverageValueOfOrdersInTimePeriodConsideringDiscountsAndCoupons([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.AverageDiscountedValueOfOrdersInTimePeriod(from, until, cancellationToken: cancellationToken);
    }
    
    [HttpGet("/AverageRevenueOfOrdersInTimePeriod")]
    public async Task<decimal> AverageValueOfOrdersInTimePeriod([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.AverageValueOfOrdersInTimePeriod(from, until, cancellationToken: cancellationToken);
    }
}