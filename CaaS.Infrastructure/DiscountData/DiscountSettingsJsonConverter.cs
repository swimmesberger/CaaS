using System.Text.Json;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base;
using Microsoft.Extensions.Options;

namespace CaaS.Infrastructure.DiscountData; 

public class DiscountSettingsJsonConverter {
    private readonly IEnumerable<DiscountComponentMetadata> _componentMetadata;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public DiscountSettingsJsonConverter(IEnumerable<DiscountComponentMetadata> componentMetadata, IOptions<DiscountJsonOptions> jsonOptions) {
        _componentMetadata = componentMetadata;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public DiscountSettingMetadata DeserializeSettings(Guid id, JsonElement actionParameters) {
        var discountComponentMetadata = _componentMetadata.FirstOrDefault(c => c.Id == id);
        if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with id '{id}'");
        var parameters = (DiscountParameters?)actionParameters.Deserialize(discountComponentMetadata.SettingsType, _jsonOptions);
        if (parameters == null) throw new ArgumentException();
        return new DiscountSettingMetadata { Id = id, Parameters = parameters };
    }
    
    public JsonElement SerializeSettings(DiscountSettingMetadata settings) {
        return JsonSerializer.SerializeToElement(settings.Parameters, _jsonOptions);
    }
}