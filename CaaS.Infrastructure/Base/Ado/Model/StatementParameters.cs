namespace CaaS.Infrastructure.Base.Ado.Model;

public record StatementParameters {
    public static readonly StatementParameters Empty = new StatementParameters();
    
    public IEnumerable<QueryParameter> Where { get; init; } = Enumerable.Empty<QueryParameter>();
    public IEnumerable<OrderParameter> OrderBy { get; init; } = Enumerable.Empty<OrderParameter>();
    public InsertParameters Insert { get; init; } = InsertParameters.Empty;
    public UpdateParameters Update { get; init; } = UpdateParameters.Empty;
    
    public StatementParameters Add(StatementParameters parameters) {
        var where = new List<QueryParameter>();
        where.AddRange(Where);
        where.AddRange(parameters.Where);
        var orderBy = new List<OrderParameter>();
        orderBy.AddRange(OrderBy);
        orderBy.AddRange(parameters.OrderBy);
        return this with {
            Where = where,
            OrderBy = orderBy,
            Insert = Insert.Add(parameters.Insert),
            Update = Update.Add(parameters.Update)
        };
    }
    
    public StatementParameters MapParameterNames(Func<string, string> selector) {
        return new StatementParameters() {
            Where = Where.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            OrderBy = OrderBy.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            Insert = Insert.MapParameterNames(selector),
            Update = Update.MapParameterNames(selector)
        };
    }

    public StatementParameters WithWhere(string name, object value) {
        return WithWhere(QueryParameter.From(name, value));
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

    public static StatementParameters CreateWhere(string name, object value)
        => Empty.WithWhere(name, value);

    public static StatementParameters CreateWhere(QueryParameter parameter) 
        => Empty.WithWhere(parameter);

    public static StatementParameters CreateWhere(IEnumerable<QueryParameter> parameters)
        => Empty.WithWhere(parameters);
}