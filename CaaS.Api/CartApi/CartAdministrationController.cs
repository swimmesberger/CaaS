using System.ComponentModel.DataAnnotations;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CartApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class CartAdministrationController : ControllerBase {
    private readonly IStatisticsService _statisticsService;

    public CartAdministrationController(IStatisticsService statisticsService) {
        _statisticsService = statisticsService;
    }
    
    [HttpGet("CartStatisticsOverall")]
    public async Task<CartStatisticsResult> CartStatisticsOverall([FromQuery] [Required] DateTimeOffset from, [FromQuery] DateTimeOffset? until = null, 
        CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until != null && until.Value.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.CartStatisticsOverall(from, until, cancellationToken: cancellationToken);
    }
    
    [HttpGet("CartStatisticsAggregatedByDate")]
    public async Task<IReadOnlyCollection<CartStatisticsResultDateAggregate>> CartStatisticsAggregatedByDate([FromQuery] [Required] DateTimeOffset from, 
        [FromQuery] DateTimeOffset? until, [Required] [FromQuery] AggregateByDatePart aggregate, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until != null && until.Value.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.CartStatisticsAggregatedByDate(from, until, aggregate, cancellationToken: cancellationToken);
    }
}
