using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Model; 

public record UpdateParameters {
    public static readonly UpdateParameters Empty = new UpdateParameters();

    public IEnumerable<string> ColumnNames { get; init; } = ImmutableList<string>.Empty;
    public IEnumerable<UpdateParameter> Values { get; init; } = ImmutableList<UpdateParameter>.Empty;
    
    public UpdateParameters MapParameterNames(Func<string, string> selector) {
        return new UpdateParameters() {
            ColumnNames = ColumnNames.Select(selector.Invoke).ToList(),
            Values = Values.Select(p => p.MapParameterNames(selector)).ToList()
        };
    }

    public UpdateParameters Add(UpdateParameters parameters) {
        var values = new List<UpdateParameter>();
        values.AddRange(Values);
        values.AddRange(parameters.Values);
        return parameters with {
            Values = values
        };
    }
}

public record UpdateParameter {
    public IEnumerable<QueryParameter> Values { get; init; } = Enumerable.Empty<QueryParameter>();
    public IEnumerable<QueryParameter> Where { get; init; } = Enumerable.Empty<QueryParameter>();
    
    public UpdateParameter MapParameterNames(Func<string, string> selector) {
        return new UpdateParameter() {
            Values = Values.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            Where = Where.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
        };
    }

    public UpdateParameter Add(UpdateParameter parameters) {
        var values = new List<QueryParameter>();
        values.AddRange(Values);
        values.AddRange(parameters.Values);
        var where = new List<QueryParameter>();
        where.AddRange(Where);
        where.AddRange(parameters.Where);
        return parameters with {
            Values = values,
            Where = where
        };
    }
}