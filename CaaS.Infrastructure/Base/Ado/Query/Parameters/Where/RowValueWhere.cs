using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;

// row value where: WHERE (x, y, ...) > (a, b, ...)
public record RowValueWhere(IEnumerable<QueryParameter> Parameters, WhereComparator Comparator = WhereComparator.Equal) : IWhereStatement {
    public static readonly RowValueWhere Empty = new RowValueWhere(Enumerable.Empty<QueryParameter>());
    
    public RowValueWhere(QueryParameter parameter, WhereComparator comparator = WhereComparator.Equal) : 
        this(ImmutableArray.Create(parameter), comparator) { }
    
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((RowValueWhere)where);
    
    public RowValueWhere Add(RowValueWhere where) {
        var parameters = new List<QueryParameter>();
        parameters.AddRange(Parameters);
        parameters.AddRange(where.Parameters);
        return this with { Parameters = parameters };
    }
    
    public RowValueWhere MapParameterNames(Func<string, string> selector) {
        return new RowValueWhere(Parameters.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList());
    }
}