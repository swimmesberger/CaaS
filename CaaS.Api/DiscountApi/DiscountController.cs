using CaaS.Api.Base.Attributes;
using CaaS.Core.DiscountAggregate.Base;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.DiscountApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class DiscountController : ControllerBase {
    private readonly IDiscountService _discountService;

    public DiscountController(IDiscountService discountService) {
        _discountService = discountService;
    }

    [HttpGet("rule")]
    public Task<IEnumerable<DiscountComponentMetadata>> GetDiscountRules() {
        return _discountService.GetDiscountRules();
    }
    
    [HttpGet("action")]
    public Task<IEnumerable<DiscountComponentMetadata>> GetDiscountActions() {
        return _discountService.GetDiscountActions();
    }
}