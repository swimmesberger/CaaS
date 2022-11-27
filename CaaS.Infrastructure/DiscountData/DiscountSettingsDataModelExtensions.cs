using System.Text.Json;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Infrastructure.DiscountData; 

public static class DiscountSettingsDataModelExtensions {
    public static DiscountSettingMetadata DeserializeAction(this DiscountSettingDataModel model, JsonSerializerOptions options)
        => options.GetSettingsConverter().GetAction(model, options);
    
    public static DiscountSettingMetadata DeserializeRule(this DiscountSettingDataModel model, JsonSerializerOptions options)
        => options.GetSettingsConverter().GetRule(model, options);
    
    public static JsonElement Serialize<T>(this T parameters, JsonSerializerOptions options) where T: DiscountParameters
        => JsonSerializer.SerializeToElement(parameters, options);

    public static JsonElement SerializeRule(this DiscountSetting discountSetting, JsonSerializerOptions options) 
        => discountSetting.Rule.Parameters.Serialize(options);
    
    public static JsonElement SerializeAction(this DiscountSetting discountSetting, JsonSerializerOptions options) 
        => discountSetting.Rule.Parameters.Serialize(options);
    
    private static DiscountSettingsJsonConverter GetSettingsConverter(this JsonSerializerOptions options)
        => options.Converters
            .Where(c => c is DiscountSettingsJsonConverter)
            .Cast<DiscountSettingsJsonConverter>()
            .First();
}