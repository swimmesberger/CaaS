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
    public async Task<CartDto?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        var result = await _cartService.GetCartById(cartId, cancellationToken);
        return _mapper.Map<CartDto>(result);
    }

    // TODO: handle as Created without value response?
    [HttpPost("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> AddProductToCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity, CancellationToken cancellationToken = default) {
        return await _cartService.AddProductToCart(cartId, productId, productQuantity, cancellationToken);
    }

    // TODO: handle as Deleted without value response?
    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        return await _cartService.RemoveProductFromCart(cartId, productId, cancellationToken);
    }

    [HttpPut("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity,
            CancellationToken cancellationToken = default) {
        return await _cartService.SetProductQuantityInCart(cartId, productId, productQuantity, cancellationToken);
    }
    
    [HttpPut("{cartId:guid}")]
    public async Task<CartDto> AddCouponToCart(Guid cartId, [FromQuery] [Required] Guid couponId, CancellationToken cancellationToken = default) {
        var result = await _cartService.AddCouponToCart(cartId, couponId, cancellationToken);
        return _mapper.Map<CartDto>(result);
    }
}