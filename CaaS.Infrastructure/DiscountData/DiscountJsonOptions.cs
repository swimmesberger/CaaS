using System.Text.Json;

namespace CaaS.Infrastructure.DiscountData; 

/// <summary>
/// Options to configure <see cref="DiscountSettingsJsonConverter"/> />.
/// </summary>
public class DiscountJsonOptions {
    public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions(JsonSerializerDefaults.Web);
}