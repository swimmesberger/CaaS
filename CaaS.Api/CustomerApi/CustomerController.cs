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
public class CustomerController : ControllerBase {
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;

    public CustomerController(ICustomerService customerService, IMapper mapper) {
        _customerService = customerService;
        _mapper = mapper;
    }

    [HttpGet("{customerId:guid}")]
    public async Task<CustomerDto?> GetById(Guid customerId, CancellationToken cancellationToken = default) {
        var result = await _customerService.GetByIdAsync(customerId, cancellationToken);
        return _mapper.Map<CustomerDto?>(result);
    }
    
    [HttpGet("{email}")]
    public async Task<CustomerDto?> GetByEmail(string email, CancellationToken cancellationToken = default) {
        var result = await _customerService.GetByEmailAsync(email, cancellationToken);
        return _mapper.Map<CustomerDto?>(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> AddCustomer([FromBody] CustomerForCreationDto customerDto, CancellationToken cancellationToken = default) {
        var customer = _mapper.Map<Customer>(customerDto);
        var savedCustomer = await _customerService.AddAsync(customer, cancellationToken);
        
        return CreatedAtAction(
            controllerName: "Customer",
            actionName: nameof(GetById),
            routeValues: new { customerId = savedCustomer.Id },
            value: _mapper.Map<CustomerDto>(savedCustomer));
    }
}