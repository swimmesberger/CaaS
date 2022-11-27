namespace CaaS.Infrastructure.Base.Ado.Model.Where;

// simple where: WHERE x > a
public record SimpleWhere(IEnumerable<QueryParameter> Parameters) : IWhereStatement {
    public static readonly SimpleWhere Empty = new SimpleWhere(Enumerable.Empty<QueryParameter>());

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((SimpleWhere)where);
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    public SimpleWhere Add(SimpleWhere where) {
        var parameters = new List<QueryParameter>();
        parameters.AddRange(Parameters);
        parameters.AddRange(where.Parameters);
        return new SimpleWhere(parameters);
    }
    
    public SimpleWhere MapParameterNames(Func<string, string> selector) {
        return new SimpleWhere(Parameters.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList());
    }
}