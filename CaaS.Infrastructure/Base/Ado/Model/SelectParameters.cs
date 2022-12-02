using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record SelectParameters(IReadOnlyList<string> Properties, bool IsAll = false) {
    public static readonly SelectParameters All = new SelectParameters(ImmutableArray<string>.Empty, true);

    public bool IsEmpty => Properties.Count <= 0;

    public SelectParameters MapParameterNames(Func<string, string> selector) {
        return this with { Properties = Properties.Select(selector).ToImmutableArray() };
    }
    
    public SelectParameters Add(SelectParameters select) {
        if (IsAll && select.IsAll) {
            return All;
        }
        if (!IsAll && select.IsAll) {
            return this;
        }
        return new SelectParameters(Properties.Concat(select.Properties).ToImmutableArray());
    }

    public static SelectParameters Create(params string[] properties) => new SelectParameters(properties);
    
    public static SelectParameters Create(IEnumerable<string> properties) => new SelectParameters(properties.ToImmutableArray());
}