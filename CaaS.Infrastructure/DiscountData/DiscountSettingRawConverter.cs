using System.Text.Json;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using Microsoft.Extensions.Options;

namespace CaaS.Infrastructure.DiscountData; 

public sealed class DiscountSettingRawConverter : IDiscountSettingRawConverter {
    private readonly DiscountJsonOptions _jsonOptions;
    private readonly IEnumerable<DiscountComponentMetadata> _componentMetadata;

    public DiscountSettingRawConverter(IOptions<DiscountJsonOptions> jsonOptions, IEnumerable<DiscountComponentMetadata> componentMetadata) {
        _jsonOptions = jsonOptions.Value;
        _componentMetadata = componentMetadata;
    }
    
    public DiscountSetting DeserializeSetting(DiscountSettingRaw rawSetting) {
        var action = DeserializeSetting(rawSetting.Action);
        var rule = DeserializeSetting(rawSetting.Rule);
        return new DiscountSetting() {
            Id = rawSetting.Id,
            Name = rawSetting.Name,
            Action = action,
            Rule = rule,
            ShopId = rawSetting.ShopId,
            ConcurrencyToken = rawSetting.ConcurrencyToken
        };
    }
    
    public DiscountSettingRaw SerializeSetting(DiscountSetting setting) {
        return new DiscountSettingRaw() {
            Id = setting.Id,
            Name = setting.Name,
            Action = SerializeSetting(setting.Action),
            Rule = SerializeSetting(setting.Rule),
            ShopId = setting.ShopId,
            ConcurrencyToken = setting.ConcurrencyToken
        };
    }

    public DiscountMetadataSettingRaw SerializeSetting(DiscountSettingMetadata metadata) {
        var componentId = metadata.Id;
        var discountComponentMetadata = _componentMetadata.FirstOrDefault(c => c.Id == componentId);
        if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with id '{componentId}'");
        return new DiscountMetadataSettingRaw() {
            Id = metadata.Id,
            Parameters = JsonSerializer.SerializeToElement(metadata.Parameters, 
                discountComponentMetadata.SettingsType, _jsonOptions.JsonSerializerOptions)
        };
    }

    public DiscountSettingMetadata DeserializeSetting(DiscountMetadataSettingRaw rawSetting) {
        DiscountComponentMetadata? discountComponentMetadata;
        if (rawSetting.Id == Guid.Empty) {
            var discountParameters = rawSetting.Parameters.Deserialize<DiscountParameters>(_jsonOptions.JsonSerializerOptions);
            discountComponentMetadata = discountParameters == null ? null : GetDiscountMetadataByName(discountParameters.Name);
            if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with by name");
        } else {
            // TODO:
            // this shouldn't be a production solution - normally the rule/action should be selected from a combobox via name but the id
            // should be associated - therefore no lookup via name is required
            // for our use-case without dynamic forms it is easier to simply provide a "text" field of the action/rule name
            // when the frontend is "advanced" enough, this can be removed
            var componentId = rawSetting.Id;
            discountComponentMetadata = _componentMetadata.FirstOrDefault(c => c.Id == componentId);
            if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with id '{componentId}'");
        }
        var parameters = (DiscountParameters?)rawSetting.Parameters.Deserialize(discountComponentMetadata.SettingsType, _jsonOptions.JsonSerializerOptions);
        if (parameters == null) throw new JsonException();
        return new DiscountSettingMetadata { Id = discountComponentMetadata.Id, Parameters = parameters };
    }
    
    private DiscountComponentMetadata? GetDiscountMetadataByName(string name) {
        return _componentMetadata
            .Where(d => {
                var param = (DiscountParameters)Activator.CreateInstance(d.SettingsType)!;
                var paramName = param.Name;
                return paramName.Equals(name);
            })
            .FirstOrDefault();
    }
}