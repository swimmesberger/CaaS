using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Order.DataModel;

[GenerateMapper]
public record ProductOrderDataModel : Base.DataModel.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid ProductId { get; init; }
    public Guid OrderId { get; init; }
    public int Amount { get; init; }
    public decimal PricePerPiece { get; init; }
}