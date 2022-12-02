using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record InsertParameters {
    public static readonly InsertParameters Empty = new InsertParameters();

    public IEnumerable<string> ColumnNames { get; init; } = ImmutableList<string>.Empty;
    public IEnumerable<IReadOnlyList<QueryParameter>> Values { get; init; } = ImmutableList<IReadOnlyList<QueryParameter>>.Empty;

    public IEnumerable<QueryParameter> Parameters {
        get => Values.SelectMany(s => s);
        init => Values = new IReadOnlyList<QueryParameter>[] { value.ToImmutableArray() };
    }

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

    public static InsertParameters CreateFromParameters(IEnumerable<QueryParameter> parameters) {
        var queryParameters = parameters.ToImmutableArray();
        return new InsertParameters() {
            Parameters = queryParameters,
            ColumnNames = queryParameters.Select(p => p.Name)
        };
    }
}