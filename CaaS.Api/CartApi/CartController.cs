using System.ComponentModel.DataAnnotations;
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

    [HttpPost("{cartId:guid}/product/{productId:guid}")]
    public async Task<ActionResult> AddProductToCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity, CancellationToken cancellationToken = default) {
        var result = await _cartService.AddProductToCartAsync(cartId, productId, productQuantity, cancellationToken);
        
        return CreatedAtAction(
            controllerName: "Cart",
            actionName: nameof(GetById),
            routeValues: new { cartId = result.Id },
            value: _mapper.Map<CartDto>(result));
    }

    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    public async Task<ActionResult> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        var result = await _cartService.RemoveProductFromCartAsync(cartId, productId, cancellationToken);
        return CreatedAtAction(
            controllerName: "Cart",
            actionName: nameof(GetById),
            routeValues: new { cartId = result.Id },
            value: _mapper.Map<CartDto>(result));
    }

    [HttpPut("{cartId:guid}/product/{productId:guid}")]
    public async Task<CartDto> SetProductQuantityInCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity,
            CancellationToken cancellationToken = default) {
        var result = await _cartService.SetProductQuantityInCartAsync(cartId, productId, productQuantity, cancellationToken);
        return _mapper.Map<CartDto>(result);
    }
    
    [HttpPut("{cartId:guid}/coupon/{couponId:guid}")]
    public async Task<CartDto> AddCouponToCart(Guid cartId, [Required] Guid couponId, CancellationToken cancellationToken = default) {
        var result = await _cartService.AddCouponToCartAsync(cartId, couponId, cancellationToken);
        return _mapper.Map<CartDto>(result);
    }
}