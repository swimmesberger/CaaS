using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
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
    public async Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByIdAsync(couponId, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByCartIdAsync(cartId, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByOrderIdAsync(orderId, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Coupon>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByCustomerIdAsync(customerId, cancellationToken);
    }
    
    public async Task<Coupon> UpdateAsync(Guid couponId, Coupon updatedCoupon, CancellationToken cancellationToken = default) {
        var oldCoupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (oldCoupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        updatedCoupon = updatedCoupon with {
            Id = couponId,
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        
        return await _couponRepository.UpdateAsync(oldCoupon, updatedCoupon, cancellationToken);
    }
    
    public async Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default) {
        if (coupon.OrderId != null || coupon.CartId != null || coupon.CustomerId != null) {
            throw new CaasValidationException("When creating a coupon orderId, cartId and customerId must be null.");
        }

        coupon = coupon with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        return await _couponRepository.AddAsync(coupon, cancellationToken);
    }
    
    public async Task DeleteAsync(Guid couponId, CancellationToken cancellationToken = default) {
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        if (coupon.OrderId != null && coupon.OrderId != Guid.Empty) {
            throw new CaasValidationException($"Cannot delete coupon '{couponId}' that is allocated to a completed order");
        }
        
        await _couponRepository.DeleteAsync(coupon, cancellationToken);
    }
    public async Task<Coupon> SetValueOfCouponAsync(Guid couponId, decimal value, CancellationToken cancellationToken = default) {
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