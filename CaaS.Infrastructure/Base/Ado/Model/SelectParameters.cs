using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record SelectParameters(IReadOnlyList<string>? Properties = null) {
    public static readonly SelectParameters Empty = new SelectParameters();

    public SelectParameters MapParameterNames(Func<string, string> selector) {
        return Properties == null ? this : new SelectParameters(Properties.Select(selector).ToImmutableArray());
    }
    
    public SelectParameters Add(SelectParameters select) {
        if (Properties == null && select.Properties == null) {
            return Empty;
        }
        if (Properties == null && select.Properties != null) {
            return select;
        }
        if (select.Properties == null && Properties != null) {
            return this;
        }
        return new SelectParameters(Properties!.Concat(select.Properties!).ToImmutableArray());
    }

    public static SelectParameters Create(params string[] properties) => new SelectParameters(properties);
}