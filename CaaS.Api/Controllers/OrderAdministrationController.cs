using CaaS.Api.Base.Attributes;
using CaaS.Core.Base;
using CaaS.Core.OrderAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class OrderAdministrationController : ControllerBase {
    private readonly IOrderService _orderService;
    private readonly IStatisticsService _statisticsService;

    public OrderAdministrationController(IOrderService orderService, IStatisticsService statisticsService) {
        _orderService = orderService;
        _statisticsService = statisticsService;
    }
    
    [HttpGet("/AverageValueOfOrdersInTimePeriodConsideringDiscountsAndCoupons")]
    public async Task<Decimal> AverageValueOfOrdersInTimePeriodConsideringDiscountsAndCoupons([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        return await _statisticsService.AverageDiscountedValueOfOrdersInTimePeriod(from, until, cancellationToken: cancellationToken);
    }
    
    [HttpGet("/AverageRevenueOfOrdersInTimePeriod")]
    public async Task<Decimal> AverageValueOfOrdersInTimePeriod([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        return await _statisticsService.AverageValueOfOrdersInTimePeriod(from, until, cancellationToken: cancellationToken);
    }
}