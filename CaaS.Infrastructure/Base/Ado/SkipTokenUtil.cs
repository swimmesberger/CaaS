using System.Text;
using System.Text.Json;
using CaaS.Core.Base;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public class SkipTokenUtil {
    public static SkipTokenValue? CreateFromRecord(IRecordValues? recordValues, IEnumerable<string> properties) {
        if (recordValues == null) return null;
        var propertyValuePairs = new Dictionary<string, object?>();
        foreach (var property in properties) {
            propertyValuePairs[property] = recordValues.GetObject(property);
        }
        return new SkipTokenValue() { PropertyValues = propertyValuePairs };
    }

    public static SkipTokenValue Parse(string value, IRecordMetadataProvider propertyMapper) {
        var propertyValuePairs = new Dictionary<string, object?>();
        var keyValuesPairs = ParseValue(value, SkipTokenValue.CommaDelimiter);
        foreach (var pair in keyValuesPairs) {
            var pieces = pair.Split(new[] { SkipTokenValue.PropertyDelimiter }, 2);
            if (pieces.Length > 1 && !string.IsNullOrWhiteSpace(pieces[0])) {
                var propertyType = propertyMapper.GetPropertyType(pieces[0]);
                var propValue = StringToValue(pieces[1], propertyType);
                propertyValuePairs.Add(pieces[0], propValue);
            } else {
                throw new ArgumentException("Failed to parse skip-token");
            }
        }
        return new SkipTokenValue() { PropertyValues = propertyValuePairs };
    }

    private static object? StringToValue(string rawValue, Type propertyType) {
        return JsonSerializer.Deserialize(rawValue, propertyType);
    }
    
    private static IList<string> ParseValue(string value, char delim) {
        IList<string> results = new List<string>();
        var escapedStringBuilder = new StringBuilder();
        for (var i = 0; i < value.Length; i++) {
            if (value[i] == '\'' || value[i] == '"') {
                escapedStringBuilder.Append(value[i]);
                var openingQuoteChar = value[i];
                i++;
                while (i < value.Length && value[i] != openingQuoteChar) {
                    escapedStringBuilder.Append(value[i++]);
                }

                if (i != value.Length) {
                    escapedStringBuilder.Append(value[i]);
                }
            } else if (value[i] == delim) {
                results.Add(escapedStringBuilder.ToString());
                escapedStringBuilder.Clear();
            } else {
                escapedStringBuilder.Append(value[i]);
            }
        }

        var lastPair = escapedStringBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(lastPair)) {
            results.Add(lastPair);
        }

        return results;
    }
}