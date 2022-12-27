using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;

namespace CaaS.Core.CouponAggregate; 

public class CouponService : ICouponService {
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public CouponService(ICouponRepository couponRepository, IUnitOfWorkManager unitOfWorkManager, ITenantIdAccessor tenantIdAccessor) {
        _couponRepository = couponRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _tenantIdAccessor = tenantIdAccessor;
    }
    public async Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByIdAsync(couponId, cancellationToken);
    }
    
    public async Task<CountedResult<Coupon>> GetByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default) {
        var items = await _couponRepository.FindByCartIdAsync(cartId, cancellationToken);
        return new CountedResult<Coupon>() { Items = items, TotalCount = await _couponRepository.CountAsync(cancellationToken) };
    }
    
    public async Task<CountedResult<Coupon>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default) {
        var items = await _couponRepository.FindByOrderIdAsync(orderId, cancellationToken);
        return new CountedResult<Coupon>() { Items = items, TotalCount = await _couponRepository.CountAsync(cancellationToken) };
    }
    
    public async Task<CountedResult<Coupon>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) {
        var items = await _couponRepository.FindByCustomerIdAsync(customerId, cancellationToken);
        return new CountedResult<Coupon>() { Items = items, TotalCount = await _couponRepository.CountAsync(cancellationToken) };
    }
    
    public async Task<Coupon> UpdateAsync(Guid couponId, Coupon updatedCoupon, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var oldCoupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (oldCoupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        updatedCoupon = updatedCoupon with {
            Id = couponId,
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        
        updatedCoupon = await _couponRepository.UpdateAsync(oldCoupon, updatedCoupon, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return updatedCoupon;
    }
    
    public async Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default) {
        if (coupon.OrderId != null || coupon.CartId != null || coupon.CustomerId != null) {
            throw new CaasValidationException("When creating a coupon orderId, cartId and customerId must be null.");
        }
        await using var uow = _unitOfWorkManager.Begin();
        coupon = coupon with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        coupon = await _couponRepository.AddAsync(coupon, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return coupon;
    }
    
    public async Task DeleteAsync(Guid couponId, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"CouponId {couponId} not found");
        }

        if (coupon.OrderId != null && coupon.OrderId != Guid.Empty) {
            throw new CaasValidationException($"Cannot delete coupon '{couponId}' that is allocated to a completed order");
        }
        
        await _couponRepository.DeleteAsync(coupon, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
    }
    public async Task<Coupon> SetValueOfCouponAsync(Guid couponId, decimal value, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
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

        updatedCoupon = await _couponRepository.UpdateAsync(coupon, updatedCoupon, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return updatedCoupon;
    }
}