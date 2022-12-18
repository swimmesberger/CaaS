using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.OrderAggregate;

namespace CaaS.Core.CouponAggregate; 

public class CouponService : ICouponService {
    private readonly ICouponRepository _couponRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public CouponService(ICouponRepository couponRepository, ICartRepository cartRepository, IOrderRepository orderRepository, ITenantIdAccessor tenantIdAccessor) {
        _couponRepository = couponRepository;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }
    public async Task<Coupon> GetCouponById(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }
        return coupon;
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByCartId(cartId, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByOrderId(orderId, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByCustomerId(customerId, cancellationToken);
    }
    
    public async Task<Coupon> UpdateCoupon(Guid couponId, Coupon updatedCoupon, CancellationToken cancellationToken = default) {
        var oldCoupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (oldCoupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        updatedCoupon = updatedCoupon with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        
        return await _couponRepository.UpdateAsync(oldCoupon, updatedCoupon, cancellationToken);
    }
    
    public async Task<Coupon> AddCoupon(Coupon coupon, CancellationToken cancellationToken = default) {
        if (coupon.OrderId != null || coupon.CartId != null || coupon.CustomerId != null) {
            throw new CaasValidationException("When creating a coupon orderId, cartId and customerId must be null.");
        }

        coupon = coupon with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        return await _couponRepository.AddAsync(coupon, cancellationToken);
    }
    
    public async Task DeleteCoupon(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        if (coupon.OrderId != null && coupon.OrderId != Guid.Empty) {
            throw new CaasValidationException($"Cannot delete coupon '{couponId}' that is allocated to a completed order");
        }
        
        await _couponRepository.DeleteAsync(coupon, cancellationToken);
    }
    public async Task<Coupon> SetValueOfCoupon(Guid couponId, decimal value, CancellationToken cancellationToken = default) {
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }
        
        if(coupon.OrderId != null || coupon.CartId != null)
        {
            throw new CaasValidationException("Value of a redeemed coupon cannot be changed");
        }

        var updatedCoupon = coupon with {
            Value = value
        };

        return await _couponRepository.UpdateAsync(coupon, updatedCoupon, cancellationToken);
    }
}