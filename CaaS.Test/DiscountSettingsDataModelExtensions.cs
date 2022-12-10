using System.Text.Json;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Test; 

public static class DiscountSettingsDataModelExtensions {
    public static JsonElement SerializeToElement<T>(this T parameters, JsonSerializerOptions options) where T: DiscountParameters
        => JsonSerializer.SerializeToElement(parameters, options);
}