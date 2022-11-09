using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record CouponDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CartId { get; init; }
    public Guid? RedeemedBy { get; init; }
}