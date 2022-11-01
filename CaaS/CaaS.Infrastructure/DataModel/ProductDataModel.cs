using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel; 

[GenerateMapper]
public record ProductDataModel : Base.DataModel {
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    [TenantIdColumn]
    public Guid ShopId { get; init; }
}