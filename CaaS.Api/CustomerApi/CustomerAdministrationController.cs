using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CustomerApi.Models;
using CaaS.Core.CustomerAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CustomerApi; 

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
    
    [HttpPost]
    public async Task<CreatedAtActionResult> AddCustomer(CustomerForCreationDto customerDto, CancellationToken cancellationToken = default) {
        var customer = _mapper.Map<Customer>(customerDto);
        var savedCustomer = await _customerService.AddCustomer(customer, cancellationToken);
        
        return CreatedAtAction(
            actionName: nameof(AddCustomer),
            routeValues: new { customerId = customer.Id },
            value: _mapper.Map<CustomerDto>(savedCustomer));
    }
    

}