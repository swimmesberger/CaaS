namespace CaaS.Infrastructure.Ado;

public record StatementParameters {
    public static readonly StatementParameters Empty = new StatementParameters();
    public IEnumerable<QueryParameter> Where { get; init; } = Enumerable.Empty<QueryParameter>();
    public IEnumerable<OrderParameter> OrderBy { get; init; } = Enumerable.Empty<OrderParameter>();
    public IEnumerable<QueryParameter> Other { get; init; } = Enumerable.Empty<QueryParameter>();

    public IEnumerable<QueryParameter> BindParameters => Where.Concat(Other);
    public IEnumerable<string> ParameterNames => Where.Select(p => p.Name)
            .Concat(OrderBy.Select(p => p.Name))
            .Concat(Other.Select(p => p.Name));

    public StatementParameters Add(StatementParameters parameters) {
        var where = new List<QueryParameter>();
        where.AddRange(Where);
        where.AddRange(parameters.Where);
        var orderBy = new List<OrderParameter>();
        orderBy.AddRange(OrderBy);
        orderBy.AddRange(parameters.OrderBy);
        var other = new List<QueryParameter>();
        other.AddRange(Other);
        other.AddRange(parameters.Other);
        return this with {
            Where = where,
            OrderBy = orderBy,
            Other = other
        };
    }

    public StatementParameters WithWhere(string name, object value) {
        return WithWhere(new QueryParameter(name, value));
    }
    
    public StatementParameters WithWhere(QueryParameter parameter) {
        return WithWhere(new[] { parameter });
    }
    
    public StatementParameters WithWhere(IEnumerable<QueryParameter> parameters) {
        return Add(new StatementParameters() { Where = parameters });
    }
    
    public StatementParameters WithOrderBy(string name, OrderType orderType = OrderType.Asc) {
        return WithOrderBy(new OrderParameter(name, orderType));
    }
    
    public StatementParameters WithOrderBy(OrderParameter parameter) {
        return WithOrderBy(new[] { parameter });
    }
    
    public StatementParameters WithOrderBy(IEnumerable<OrderParameter> parameters) {
        return Add(new StatementParameters() { OrderBy = parameters });
    }

    public StatementParameters MapParameterNames(Func<string, string> selector) {
        return new StatementParameters() {
            Where = Where.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            OrderBy = OrderBy.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            Other = Where.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList()
        };
    }

    public static StatementParameters CreateWhere(string name, object value)
        => Empty.WithWhere(name, value);

    public static StatementParameters CreateWhere(QueryParameter parameter) 
        => Empty.WithWhere(parameter);

    public static StatementParameters CreateWhere(IEnumerable<QueryParameter> parameters)
        => Empty.WithWhere(parameters);
}