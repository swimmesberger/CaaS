using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Validation;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.CartAggregate; 

public class CartService : ICartService {
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IDiscountService _discountService;
    private readonly ICouponService _couponService;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly ISystemClock _timeProvider;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository, 
        IShopRepository shopRepository, IDiscountService discountService, ICouponService couponService, IUnitOfWorkManager unitOfWorkManager,
        ITenantIdAccessor tenantIdAccessor,  ISystemClock timeProvider) {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _couponService = couponService;
        _discountService = discountService;
        _unitOfWorkManager = unitOfWorkManager;
        _tenantIdAccessor = tenantIdAccessor;
        _timeProvider = timeProvider;
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) return null;
        var updatedCart = cart with { LastAccess = _timeProvider.UtcNow };
        cart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        cart = await PostProcessCart(cart, cancellationToken);
        return cart;
    }

    public async Task<Cart> UpdateCartAsync(Cart userCart, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var oldCart = await _cartRepository.FindByIdAsync(userCart.Id, cancellationToken);
        var cartItems = new List<CartItem>();
        foreach (var cartItem in userCart.Items) {
            if (cartItem.Amount <= 0) {
                throw new CaasValidationException(new ValidationFailure(nameof(cartItem.Amount), "Invalid product quantity"));
            }
            var product = await _productRepository.FindByIdAsync(cartItem.Product.Id, cancellationToken);
            if (product == null) {
                throw new CaasItemNotFoundException($"product {cartItem.Product.Id} not found");
            }
            var oldItemIdx = oldCart?.Items.FindIndex(i => i.Product.Id.Equals(product.Id)) ?? -1;
            if (oldItemIdx >= 0) {
                cartItems.Add(oldCart!.Items[oldItemIdx] with {
                    Amount = cartItem.Amount
                });
            } else {
                cartItems.Add(new CartItem() {
                    Product = product,
                    ShopId = _tenantIdAccessor.GetTenantGuid(),
                    CartId = userCart.Id,
                    Amount = cartItem.Amount
                });
            }
        }
        var coupons = await _couponService
            .RedeemCouponsAsync(oldCart?.Coupons ?? ImmutableArray<Coupon>.Empty, userCart.Coupons, userCart.Id, 
                userCart.Customer?.Id, cancellationToken);

        // init empty cart
        var cart = new Cart() {
            Id = userCart.Id,
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = userCart.Customer,
            Items = cartItems.ToImmutableList(),
            Coupons = coupons,
            LastAccess = _timeProvider.UtcNow,
            ConcurrencyToken = oldCart?.ConcurrencyToken ?? string.Empty
        };
        try {
            if (oldCart == null) {
                cart = await _cartRepository.AddAsync(cart, cancellationToken);
            } else {
                cart = await _cartRepository.UpdateAsync(oldCart, cart, cancellationToken);
            }
            cart = (await _cartRepository.FindByIdAsync(cart.Id, cancellationToken))!;
            cart = await PostProcessCart(cart, cancellationToken);
            if (cart.TotalPrice < 0) {
                throw new CaasValidationException("Cannot add coupon because cart value would be negative");
            }
            await uow.CompleteAsync(cancellationToken);
            return cart;
        } catch (CaasConstraintViolationDbException constraintViolationException) {
            if ("customer_e_mail_key".Equals(constraintViolationException.ConstraintName)) {
                throw new CaasDuplicateCustomerEmailException($"The e-mail address {userCart.Customer?.EMail} is already taken") {
                    Type = constraintViolationException.ConstraintName
                };
            }
            throw;
        }
    }

    public async Task<Cart> SetCustomerAsync(Guid cartId, Customer? customer, CancellationToken cancellationToken = default) {
        var oldCart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (oldCart == null) {
            throw new CaasItemNotFoundException($"Cart '{cartId}' not found");
        }
        Cart cart;
        if (customer != null) {
            if (oldCart.Customer != null) {
                throw new CaasValidationException("Cart customer must be null if customer is provided");
            }

            cart = oldCart with {
                Customer = customer
            };
            cart = await _cartRepository.UpdateAsync(oldCart, cart, cancellationToken);
        } else if(oldCart.Customer != null) {
            cart = oldCart;
        } else {
            throw new CaasValidationException("Cart needs a customer");
        }
        return cart;
    }

    public async Task<int> DeleteExpiredCartsAsync(CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var cartLifetime = await _shopRepository.FindCartLifetimeByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (cartLifetime == null) {
            throw new CaasValidationException(new ValidationFailure("tenantId", "Invalid tenant id"));
        }
        var expiredCarts = await _cartRepository.FindExpiredCartsAsync(cartLifetime.Value, cancellationToken);
        await _cartRepository.DeleteAsync(expiredCarts, cancellationToken);
        var deletedCount = expiredCarts.Count;
        await uow.CompleteAsync(cancellationToken);
        return deletedCount;
    }

    public async Task DeleteAsync(Guid cartId, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        await _cartRepository.DeleteAsync(cart, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = default) {
        return _cartRepository.CountAsync(cancellationToken);
    }

    private async Task<Cart> PostProcessCart(Cart cart, CancellationToken cancellationToken = default) {
        return await _discountService.ApplyDiscountAsync(cart, cancellationToken);
    }
}