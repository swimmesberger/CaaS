using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CustomerApi.Models;
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
    
    [HttpGet("{orderId:guid}")]
    public async Task<OrderDto?> GetOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var result = await _orderService.FindOrderById(orderId, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }

    [HttpPost("createOrderWithCustomer/{cartId:guid}")]
    public async Task<OrderDto> CreateOrderFromCart(Guid cartId, [FromBody][Required] OrderForCreationDto orderDto, CancellationToken cancellationToken = default) {
        var customer = _mapper.Map<Customer>(orderDto.Customer);
        var result = await _orderService.CreateOrderFromCart(cartId, customer, orderDto.BillingAddress, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
    
    [HttpPost("createOrderWithoutCustomer/{cartId:guid}")]
    public async Task<OrderDto> CreateOrderFromCart(Guid cartId, [FromBody][Required] Address billingAddress, CancellationToken cancellationToken = default) {
        var result = await _orderService.CreateOrderFromCart(cartId, billingAddress, cancellationToken);
        return _mapper.Map<OrderDto>(result);
    }
}