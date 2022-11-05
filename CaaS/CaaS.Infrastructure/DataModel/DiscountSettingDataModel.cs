using System.Text.Json;
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record DiscountSettingDataModel() : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid Rule { get; init; }
    public Guid Action { get; init; }
    public JsonDocument? RuleParameters { get; init; } = null;
    public JsonDocument? ActionParameters { get; init; } = null;
}