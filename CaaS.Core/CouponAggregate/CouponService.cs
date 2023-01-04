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

    public async Task<Coupon?> GetByCodeAsync(string couponCode, CancellationToken cancellationToken = default) {
        return await _couponRepository.FindByCodeAsync(couponCode, cancellationToken);
    }

    public async Task<CountedResult<Coupon>> GetByAsync(CouponQuery couponQuery, CancellationToken cancellationToken = default) {
        var items = await _couponRepository.FindByAsync(couponQuery, cancellationToken);
        return new CountedResult<Coupon>() { Items = items, TotalCount = await _couponRepository.CountAsync(cancellationToken) };
    }

    public async Task<Coupon> UpdateAsync(Coupon updatedCoupon, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var oldCoupon = await _couponRepository.FindByIdAsync(updatedCoupon.Id, cancellationToken);
        if (oldCoupon == null) {
            throw new CaasItemNotFoundException($"CouponId {updatedCoupon.Id} not found");
        }
        if(oldCoupon.Value != updatedCoupon.Value && (oldCoupon.OrderId != null || oldCoupon.CartId != null)) {
            throw new CaasValidationException("Value of a redeemed coupon cannot be changed");
        }
        if(oldCoupon.Code != updatedCoupon.Code && (oldCoupon.OrderId != null || oldCoupon.CartId != null)) {
            throw new CaasValidationException("Code of a redeemed coupon cannot be changed");
        }
        updatedCoupon = oldCoupon with {
            Code = updatedCoupon.Code,
            Value = updatedCoupon.Value,
            OrderId = updatedCoupon.OrderId,
            CartId = updatedCoupon.CartId,
            CustomerId = updatedCoupon.CustomerId
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
    
    public async Task<IReadOnlyList<Coupon>> RedeemCouponsAsync(IEnumerable<Coupon> oldDomainModels, IReadOnlyCollection<Coupon> newDomainModels, 
        Guid cartId, Guid? customerId = null, CancellationToken cancellationToken = default) {
        newDomainModels = await FindUserCoupons(newDomainModels, cancellationToken);
        var changeTracker = ChangeTracker.CreateDiff(oldDomainModels, newDomainModels);
        var currentCoupons = new List<Coupon>();
        foreach (var addedCoupon in changeTracker.AddedItems) {
            currentCoupons.Add(await ApplyCouponAsync(addedCoupon, cartId, customerId, cancellationToken));
        }
        foreach (var removedCoupon in changeTracker.RemovedItems) {
            await UnapplyCouponAsync(removedCoupon, cartId, cancellationToken);
        }
        return currentCoupons;
    }

    private async Task<Coupon> ApplyCouponAsync(Coupon coupon, Guid cartId, Guid? customerId = null, CancellationToken cancellationToken = default) {
        if (coupon.OrderId != null) {
            throw new CaasValidationException($"Coupon '{coupon.Code}' was already redeemed by order '{coupon.OrderId}'");
        }

        var updatedCoupon = coupon with {
            CartId = cartId,
            CustomerId = customerId
        };
        return await _couponRepository.UpdateAsync(coupon, updatedCoupon, cancellationToken);
    }

    private async Task<Coupon> UnapplyCouponAsync(Coupon coupon, Guid cartId, CancellationToken cancellationToken = default) {
        if (coupon.OrderId != null) {
            throw new CaasValidationException($"Coupon '{coupon.Code}' was already redeemed by order '{coupon.OrderId}'");
        }
        if (!coupon.CartId.Equals(cartId)) {
            throw new CaasValidationException($"Can't unapply '{coupon.Code}'");
        }
        var updatedCoupon = coupon with {
            CartId = null,
            CustomerId = null
        };
        return await _couponRepository.UpdateAsync(coupon, updatedCoupon, cancellationToken);
    }

    private async Task<IReadOnlyList<Coupon>> FindUserCoupons(IReadOnlyCollection<Coupon> userCoupons, CancellationToken cancellationToken = default) {
        var foundCoupons = new List<Coupon>(userCoupons.Count);
        foreach (var userCoupon in userCoupons) {
            foundCoupons.Add(await FindUserCoupon(userCoupon, cancellationToken));
        }
        return foundCoupons;
    }

    private async Task<Coupon> FindUserCoupon(Coupon userCoupon, CancellationToken cancellationToken = default) {
        Coupon? coupon;
        if (userCoupon.Id != Guid.Empty) {
            coupon = await _couponRepository.FindByIdAsync(userCoupon.Id, cancellationToken);
            if (coupon == null) {
                throw new CaasItemNotFoundException($"Coupon {userCoupon.Id} not found");
            }
        } else if(!string.IsNullOrEmpty(userCoupon.Code)) {
            coupon = await _couponRepository.FindByCodeAsync(userCoupon.Code, cancellationToken);
            if (coupon == null) {
                throw new CaasItemNotFoundException($"Coupon {userCoupon.Code} not found");
            }
        } else {
            throw new CaasValidationException("Invalid coupon passed");
        }
        return coupon;
    }
}