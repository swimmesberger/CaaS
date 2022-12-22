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
    
    [HttpPost("createOrderWithCustomer/{cartId:guid}")]
    public async Task<OrderDto> CreateOrderFromCart(Guid cartId, [FromBody][Required] OrderForCreationDto orderDto, CancellationToken cancellationToken = default) {
        var result = await _orderService.CreateOrderFromCartAsync(cartId, orderDto.CustomerId, orderDto.BillingAddress, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
    
    [HttpPost("createOrderWithoutCustomer/{cartId:guid}")]
    public async Task<OrderDto> CreateOrderFromCart(Guid cartId, [FromBody][Required] Address billingAddress, CancellationToken cancellationToken = default) {
        var result = await _orderService.CreateOrderFromCartAsync(cartId, billingAddress, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
}