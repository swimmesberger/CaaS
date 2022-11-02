using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record DiscountSettingDataModel() : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = String.Empty;
    public Guid Rule { get; init; }
    public Guid Action { get; init; }
    public string RuleParameters { get; init; } = String.Empty;
    public string ActionParameters { get; init; } = String.Empty;
}