using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.CartApi.Models;
using CaaS.Core.CartAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CartApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class CartController : ControllerBase {
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    public CartController(ICartService cartService, IMapper mapper) {
        _cartService = cartService;
        _mapper = mapper;
    }

    [HttpGet("{cartId:guid}")]
    public async Task<CartDto?> GetById(Guid cartId, CancellationToken cancellationToken = default) {
        var result = await _cartService.GetByIdAsync(cartId, cancellationToken);
        return _mapper.Map<CartDto?>(result);
    }
    
    [HttpPost("{cartId:guid}")]
    public async Task<CartDto?> UpdateCart([FromRoute] Guid cartId, [FromBody] CartForUpdateDto cartDto, CancellationToken cancellationToken = default) {
        var updatedCart = _mapper.Map<Cart>(cartDto);
        updatedCart = updatedCart with { Id = cartId };
        var result = await _cartService.UpdateCartAsync(updatedCart, cancellationToken);
        return _mapper.Map<CartDto?>(result);
    }
}