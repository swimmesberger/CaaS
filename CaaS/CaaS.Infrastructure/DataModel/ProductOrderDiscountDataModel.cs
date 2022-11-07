
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record ProductOrderDiscountDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }

    public string DiscountName { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public Guid ProductOrderId { get; init; }
}