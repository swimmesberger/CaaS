﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CartApi.Models;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.CartAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CartApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
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
    
    [HttpGet("/CartStatisticsOverall")]
    public async Task<CartStatisticsResult> CartStatisticsOverall([FromQuery] [Required] DateTimeOffset from, [FromQuery] [Required] DateTimeOffset until, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.CartStatisticsOverall(from, until, cancellationToken: cancellationToken);
    }
    
    [HttpGet("/CartStatisticsAggregatedByDate")]
    public async Task<IReadOnlyCollection<CartStatisticsResultDateAggregate>> CartStatisticsAggregatedByDate([FromQuery] [Required] DateTimeOffset from, 
        [FromQuery] [Required] DateTimeOffset until, [Required] [FromQuery] AggregateByDatePart aggregate, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.CartStatisticsAggregatedByDate(from, until, aggregate, cancellationToken: cancellationToken);
    }
    
}
