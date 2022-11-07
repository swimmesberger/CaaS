using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record OrderDiscountDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }

    public string DiscountName { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public Guid OrderId { get; init; }
}