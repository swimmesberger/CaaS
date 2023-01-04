using AutoMapper;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CustomerApi.Models;
using CaaS.Core.CustomerAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CustomerApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class CustomerAdministrationController : ControllerBase {
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;

    public CustomerAdministrationController(ICustomerService customerService, IMapper mapper) {
        _customerService = customerService;
        _mapper = mapper;
    }

    [HttpPut("{customerId:guid}")]
    public async Task<CustomerDto> UpdateCustomer(Guid customerId, [FromBody] CustomerForUpdateDto customerDto, CancellationToken cancellationToken = default) {
        var updatedCustomer = _mapper.Map<Customer>(customerDto);
        var result = await _customerService.UpdateAsync(customerId, updatedCustomer, cancellationToken);
        return _mapper.Map<CustomerDto>(result);
    }
    
    [HttpDelete("{customerId:guid}")]
    public async Task<ActionResult> DeleteCustomer(Guid customerId, CancellationToken cancellationToken = default) {
        await _customerService.DeleteAsync(customerId, cancellationToken);
        return NoContent();
    }
}