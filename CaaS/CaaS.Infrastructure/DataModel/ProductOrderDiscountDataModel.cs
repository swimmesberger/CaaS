
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record ProductOrderDiscountDataModel : Base.DataModel {
    public string DiscountName { get; init; }
    public decimal Discount { get; init; }
    public Guid ProductOrderId { get; init; }
    public Guid ShopId { get; init; }
}