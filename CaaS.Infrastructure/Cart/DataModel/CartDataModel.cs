using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Cart.DataModel;

[GenerateMapper]
public record CartDataModel : Base.DataModel.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffset.UtcNow;
}