using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record OrderDiscountDataModel : Base.DataModel {
    public string DiscountName { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public Guid OrderId { get; init; }
    public Guid ShopId { get; init; }
}