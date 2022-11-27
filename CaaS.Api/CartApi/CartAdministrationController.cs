using CaaS.Api.Base.Attributes;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CartApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class CartAdministrationController : ControllerBase {
    private readonly ICartService _cartService;
    private readonly IStatisticsService _statisticsService;

    public CartAdministrationController(ICartService cartService, IStatisticsService statisticsService) {
        _cartService = cartService;
        _statisticsService = statisticsService;
    }

    [HttpGet("/CartStatistics")]
    public async Task<CartStatisticsResult> GetCartStatistics([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        return await _statisticsService.GetCartStatistics(from, until, cancellationToken: cancellationToken);
    }
}
