using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Order.DataModel;

[GenerateMapper]
public record OrderDataModel : Base.DataModel.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.Now;
}