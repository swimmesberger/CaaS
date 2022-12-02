using CaaS.Infrastructure.Base.Ado.Model.Where;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record StatementParameters {
    public static readonly StatementParameters Empty = new StatementParameters();
    
    public SelectParameters Select = SelectParameters.Empty;
    public WhereParameters Where { get; init; } = WhereParameters.Empty;
    public IEnumerable<OrderParameter> OrderBy { get; init; } = Enumerable.Empty<OrderParameter>();
    public InsertParameters Insert { get; init; } = InsertParameters.Empty;
    public UpdateParameters Update { get; init; } = UpdateParameters.Empty;
    public long? Limit { get; init; }
    
    public StatementParameters Add(StatementParameters parameters) {
        var orderBy = new List<OrderParameter>();
        orderBy.AddRange(OrderBy);
        orderBy.AddRange(parameters.OrderBy);
        return this with {
            Select = Select.Add(parameters.Select),
            Where = Where.Add(parameters.Where),
            OrderBy = orderBy,
            Insert = Insert.Add(parameters.Insert),
            Update = Update.Add(parameters.Update),
            Limit = Limit ?? parameters.Limit
        };
    }
    
    public StatementParameters MapParameterNames(Func<string, string> selector) {
        return new StatementParameters() {
            Select = Select.MapParameterNames(selector),
            Where = Where.MapParameterNames(selector),
            OrderBy = OrderBy.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            Insert = Insert.MapParameterNames(selector),
            Update = Update.MapParameterNames(selector)
        };
    }

    public StatementParameters WithSelect(params string[] properties) {
        return Add(new StatementParameters() { Select = new SelectParameters(properties) });
    }

    public StatementParameters WithWhere(string name, object value) {
        return WithWhere(QueryParameter.From(name, value));
    }
    
    public StatementParameters WithWhere(QueryParameter parameter) {
        return WithWhere(new[] { parameter });
    }
    
    public StatementParameters WithWhere(IEnumerable<QueryParameter> parameters) {
        return Add(new StatementParameters() { Where = WhereParameters.CreateFromParameters(parameters) });
    }
    
    public StatementParameters WithWhere(IWhereStatement whereStatement) {
        return Add(new StatementParameters() { Where = WhereParameters.Create(whereStatement) });
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
    
    public StatementParameters WithLimit(long limit) {
        return Add(new StatementParameters() { Limit = limit });
    }

    public static StatementParameters CreateWhere(string name, object value)
        => Empty.WithWhere(name, value);

    public static StatementParameters CreateWhere(QueryParameter parameter) 
        => Empty.WithWhere(parameter);

    public static StatementParameters CreateWhere(IEnumerable<QueryParameter> parameters)
        => Empty.WithWhere(parameters);
    
    public static StatementParameters CreateWhere(IWhereStatement whereStatement)
        => Empty.WithWhere(whereStatement);
}