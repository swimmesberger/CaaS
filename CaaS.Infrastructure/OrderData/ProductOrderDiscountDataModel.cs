
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.OrderData;

[GenerateMapper]
public record ProductOrderDiscountDataModel : DataModel {
    public string DiscountName { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public Guid ProductOrderId { get; init; }
    public Guid ShopId { get; init; }
}