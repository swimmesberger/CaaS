using System.Collections.Immutable;
using System.Net;
using System.Text;

namespace CaaS.Core.Base.Pagination;

public class SkipTokenValue {
    public const char CommaDelimiter = ',';
    public const char PropertyDelimiter = ':';

    public IReadOnlyDictionary<string, object?> PropertyValues { get; init; } = ImmutableDictionary<string, object?>.Empty;

    public override string ToString() {
        var sb = new StringBuilder();
        var first = false;
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