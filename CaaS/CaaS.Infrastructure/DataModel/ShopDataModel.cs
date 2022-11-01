using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record ShopDataModel : Base.DataModel {
    public string Name { get; init; } = string.Empty;
}