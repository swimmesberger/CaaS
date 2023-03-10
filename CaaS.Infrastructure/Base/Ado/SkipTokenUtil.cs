using System.Net;
using System.Text;
using System.Text.Json;
using CaaS.Core.Base.Pagination;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public static class SkipTokenUtil {
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
                var rawValue = pieces[1];
                rawValue = WebUtility.UrlDecode(rawValue);
                var propValue = StringToValue(rawValue, propertyType);
                propertyValuePairs.Add(pieces[0], propValue);
            } else {
                throw new ArgumentException("Failed to parse skip-token");
            }
        }
        return new SkipTokenValue() { PropertyValues = propertyValuePairs };
    }

    private static object? StringToValue(string rawValue, Type propertyType) {
        if (typeof(string) == propertyType) {
            return rawValue;
        } else if (typeof(int) == propertyType) {
            return int.Parse(rawValue);
        } else if (typeof(double) == propertyType) {
            return double.Parse(rawValue);
        } else if (typeof(decimal) == propertyType) {
            return decimal.Parse(rawValue);
        } else if (typeof(long) == propertyType) {
            return long.Parse(rawValue);
        } else if (typeof(short) == propertyType) {
            return short.Parse(rawValue);
        } else if (typeof(float) == propertyType) {
            return float.Parse(rawValue);
        } else if (typeof(Guid) == propertyType) {
            return Guid.Parse(rawValue);
        }
        throw new NotImplementedException("Unsupported type " + propertyType);
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