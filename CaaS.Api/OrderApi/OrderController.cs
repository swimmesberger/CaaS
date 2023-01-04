using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.OrderApi.Models;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.OrderAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.OrderApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class OrderController : ControllerBase {
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    
    public OrderController(IOrderService orderService, IMapper mapper) {
        _orderService = orderService;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<OrderDto> CreateOrderFromCart([FromBody][Required] OrderForCreationDto orderDto, CancellationToken cancellationToken = default) {
        var customer = _mapper.Map<Customer>(orderDto.Customer);
        var result = await _orderService.CreateOrderFromCartAsync(orderDto.CartId, orderDto.BillingAddress, customer, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
}