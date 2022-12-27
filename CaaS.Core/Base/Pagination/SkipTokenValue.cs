using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaaS.Core.Base.Pagination;

[JsonConverter(typeof(SkipTokenValueJsonConverter))]
public class SkipTokenValue {
    public const char CommaDelimiter = ',';
    public const char PropertyDelimiter = ':';

    public IReadOnlyDictionary<string, object?> PropertyValues { get; init; } = ImmutableDictionary<string, object?>.Empty;

    public override string ToString() {
        var sb = new StringBuilder();
        var first = true;
        foreach (var entry in PropertyValues) {
            if (first) {
                first = false;
            } else {
                sb.Append(CommaDelimiter);
            }
            sb.Append(entry.Key);
            sb.Append(PropertyDelimiter);
            sb.Append(WebUtility.UrlEncode(entry.Value?.ToString()));
        }
        return sb.ToString();
    }
}

public sealed class SkipTokenValueJsonConverter : JsonConverter<SkipTokenValue> {
    public override SkipTokenValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }
    
    public override void Write(Utf8JsonWriter writer, SkipTokenValue value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }
}