using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record CartDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffset.UtcNow;
}