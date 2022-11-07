using System.Collections.Immutable;

namespace CaaS.Infrastructure.Ado.Model;

public record InsertParameters {
    public static readonly InsertParameters Empty = new InsertParameters();

    public IEnumerable<string> ColumnNames { get; init; } = ImmutableList<string>.Empty;
    public IEnumerable<IReadOnlyList<QueryParameter>> Values { get; init; } = ImmutableList<IReadOnlyList<QueryParameter>>.Empty;

    public InsertParameters MapParameterNames(Func<string, string> selector) {
        return new InsertParameters() {
            ColumnNames = ColumnNames.Select(selector.Invoke).ToList(),
            Values = Values.Select(pars => pars.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList()).ToList()
        };
    }

    public InsertParameters Add(InsertParameters parameters) {
        var values = new List<IReadOnlyList<QueryParameter>>();
        values.AddRange(Values);
        values.AddRange(parameters.Values);
        return parameters with {
            Values = values
        };
    }
}