using System.Text.Json;
using System.Text.Json.Serialization;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Infrastructure.DiscountData; 

public class DiscountSettingsJsonConverter : JsonConverter<DiscountSettingMetadata> {
    private readonly IEnumerable<DiscountComponentMetadata> _componentMetadata;

    public DiscountSettingsJsonConverter(IEnumerable<DiscountComponentMetadata> componentMetadata) {
        _componentMetadata = componentMetadata;
    }
    
    public override DiscountSettingMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException();
        }
        var componentId = GetComponentId(ref reader);
        var parameters = GetParameters(ref reader, componentId, options);
        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject) {
            throw new JsonException();
        }
        return new DiscountSettingMetadata { Id = componentId, Parameters = parameters };
    }

    public override void Write(Utf8JsonWriter writer, DiscountSettingMetadata value, JsonSerializerOptions options) {
        var componentId = value.Id;
        var discountComponentMetadata = _componentMetadata.FirstOrDefault(c => c.Id == componentId);
        if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with id '{componentId}'");
        writer.WriteStartObject();
        writer.WriteString("Id", componentId.ToString());
        writer.WritePropertyName("Parameters");
        JsonSerializer.Serialize(writer, value.Parameters, discountComponentMetadata.SettingsType, options);
        writer.WriteEndObject();
    }
    
    private Guid GetComponentId(ref Utf8JsonReader reader) {
        MoveToProperty(ref reader, nameof(DiscountSettingMetadata.Id));
        if (!reader.Read() || reader.TokenType != JsonTokenType.String) {
            throw new JsonException();
        }
        var componentId = reader.GetString();
        if (componentId == null) throw new JsonException();
        return Guid.Parse(componentId);
    }

    private DiscountParameters GetParameters(ref Utf8JsonReader reader, Guid componentId, JsonSerializerOptions options) {
        MoveToProperty(ref reader, nameof(DiscountSettingMetadata.Parameters));
        var discountComponentMetadata = _componentMetadata.FirstOrDefault(c => c.Id == componentId);
        if (discountComponentMetadata == null) throw new ArgumentException($"Can't find action with id '{componentId}'");
        var parameters = (DiscountParameters?)JsonSerializer.Deserialize(ref reader, discountComponentMetadata.SettingsType, options);
        if (parameters == null) throw new JsonException();
        return parameters;
    }

    private static void MoveToProperty(ref Utf8JsonReader reader, string propertyName) {
        while (reader.Read()) {
            if (reader.TokenType != JsonTokenType.PropertyName) {
                throw new JsonException();
            }
            var name = reader.GetString();
            if (name == null || !name.Equals(propertyName)) continue;
            break;
        }
    }
}