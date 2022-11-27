namespace CaaS.Infrastructure.Base.Ado.Model.Where;

public record WhereParameters(IEnumerable<IWhereStatement> Statements) : IWhereStatement {
    public static readonly WhereParameters Empty = new WhereParameters(Enumerable.Empty<IWhereStatement>());

    public IEnumerable<QueryParameter> Parameters => Statements.SelectMany(s => s.Parameters);
    
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((WhereParameters)where);
    

    public WhereParameters Add(WhereParameters where) {
        var statements = new List<IWhereStatement>();
        statements.AddRange(Statements);
        statements.AddRange(where.Statements);
        return Create(statements);
    }
    
    public WhereParameters Add(IWhereStatement statement) {
        var statements = new List<IWhereStatement>();
        statements.AddRange(Statements);
        statements.Add(statement);
        return Create(statements);
    }

    public WhereParameters MapParameterNames(Func<string, string> selector) 
        => new WhereParameters(Statements.Select(s => s.MapParameterNames(selector)).ToList());
    
    public WhereParameters Simplify() {
        var simpleWhere = SimpleWhere.Empty;
        var rowValueWhere = RowValueWhere.Empty;
        var searchWhere = SearchWhere.Empty;
        foreach (var whereStatement in Statements) {
            switch (whereStatement) {
                case SimpleWhere addedSimpleWhere:
                    simpleWhere = simpleWhere.Add(addedSimpleWhere);
                    break;
                case RowValueWhere addedCompositeWhere:
                    rowValueWhere = rowValueWhere.Add(addedCompositeWhere);
                    break;
                case SearchWhere addedSearchWhere:
                    searchWhere = searchWhere.Add(addedSearchWhere);
                    break;
                default:
                    throw new ArgumentException($"Can't find where statement type {whereStatement.GetType()}");
            }
        }
        var whereStatements = new List<IWhereStatement>();
        if (simpleWhere.Parameters.Any()) {
            whereStatements.Add(simpleWhere);
        }
        if (rowValueWhere.Parameters.Any()){
            whereStatements.Add(rowValueWhere);
        }
        if (searchWhere.Parameters.Any()) {
            whereStatements.Add(searchWhere);
        }
        if (!whereStatements.Any()) {
            return Empty;
        }
        return new WhereParameters(whereStatements);
    }

    public static WhereParameters Create(IEnumerable<IWhereStatement> statements) 
        => new WhereParameters(statements).Simplify();

    public static WhereParameters Create(IWhereStatement statement)
        => Create(new[] { statement });
    
    public static WhereParameters CreateFromParameters(IEnumerable<QueryParameter> parameters) 
        => new WhereParameters(new [] { new SimpleWhere(parameters) });
    
    public static WhereParameters CreateFromParameters(string name, object value)
        => CreateFromParameter(QueryParameter.From(name, value));

    public static WhereParameters CreateFromParameter(QueryParameter parameter) 
        => CreateFromParameters(new[] { parameter });
}