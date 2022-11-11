using CaaS.Infrastructure.Base.DataMapping;
using CaaS.Infrastructure.Base.DataModel;

namespace CaaS.Infrastructure.Coupon;

[GenerateMapper]
public record CouponDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CartId { get; init; }
    public Guid? RedeemedBy { get; init; }
}