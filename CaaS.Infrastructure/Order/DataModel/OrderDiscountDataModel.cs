using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Order.DataModel;

[GenerateMapper]
public record OrderDiscountDataModel : Base.DataModel.DataModel {
    public string DiscountName { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public Guid OrderId { get; init; }
    public Guid ShopId { get; init; }
}