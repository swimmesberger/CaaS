using AutoMapper;
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
    private readonly IMapper _mapper;

    public CartAdministrationController(ICartService cartService, IStatisticsService statisticsService, IMapper mapper) {
        _cartService = cartService;
        _statisticsService = statisticsService;
        _mapper = mapper;
    }

    [HttpGet("/CartStatistics")]
    public async Task<CartStatisticsResult> GetCartStatistics([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        return await _statisticsService.GetCartStatistics(from, until, cancellationToken: cancellationToken);
    }
    
}
