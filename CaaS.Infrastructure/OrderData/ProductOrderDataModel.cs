using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.OrderData;

[GenerateMapper]
public record ProductOrderDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid ProductId { get; init; }
    public Guid OrderId { get; init; }
    public int Amount { get; init; }
    public decimal PricePerPiece { get; init; }
}