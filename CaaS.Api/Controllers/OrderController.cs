using CaaS.Api.Base.Attributes;
using CaaS.Core.OrderAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class OrderController : ControllerBase {
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService) {
        _orderService = orderService;
    }
}