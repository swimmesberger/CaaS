using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Shop.DataModel;

[GenerateMapper]
public record ShopDataModel : Base.DataModel.DataModel {
    public string Name { get; init; } = string.Empty;
    public int CartLifetimeMinutes { get; init; } = Core.Shop.Entities.Shop.DefaultCartLifetimeMinutes;
    public Guid AdminId { get; init; }
}