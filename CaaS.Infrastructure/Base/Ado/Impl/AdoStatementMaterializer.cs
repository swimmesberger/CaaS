using System.Collections;
using System.Text;
using CaaS.Core.Base.Exceptions;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public class AdoStatementMaterializer : IStatementSqlGenerator {
    private const string LimitParamName = "limitParamName";
    
    public MaterializedStatements MaterializeStatement(Statement statement) {
        // map all parameters to column name schema
        statement = statement.MapToColumnNames();
        return statement.Type switch {
            StatementType.Count => new MaterializedStatements(MaterializeCount(statement)),
            StatementType.Create => new MaterializedStatements(MaterializeInsert(statement)),
            StatementType.Update => MaterializeUpdate(statement),
            StatementType.Delete => new MaterializedStatements(MaterializeDelete(statement)),
            StatementType.Find => new MaterializedStatements(MaterializeFind(statement)),
            _ => throw new ArgumentException()
        };
    }

    private MaterializedStatement MaterializeInsert(Statement statement) {
        var sql = $"INSERT INTO {statement.GetTableName()} ({statement.GetInsertColumns()}) VALUES {statement.GetInsertParameters()}";
        var parameters = statement.Parameters.InsertParameters.Values.SelectMany((values, idx) => {
            return values.Select(q => q with { ParameterName = $"{q.ParameterName}_{idx}" });
        });
        return new MaterializedStatement(sql) {
            Parameters = parameters
        };
    }

    private MaterializedStatements MaterializeUpdate(Statement statement) {
        return new MaterializedStatements(statement.Parameters.Update.Values.Select(p => MaterializeUpdate(statement.GetTableName(), p)).ToList());
    }

    private MaterializedStatement MaterializeUpdate(string tableName, UpdateParameter updateParameter) {
        var sql = new StringBuilder("UPDATE");
        sql.Append(' ').Append(tableName);
        sql.Append(" SET");
        var first = true;
        foreach (var parameter in updateParameter.Values) {
            if (first) {
                first = false;
            } else {
                sql.Append(',');
            }
            sql.Append(' ').Append(parameter.Name).Append(" = ").Append('@').Append(parameter.ParameterName);
        }
        var parameters = new List<QueryParameter>();
        var whereParams = CreateWhereClause(sql, updateParameter.Where);
        parameters.AddRange(updateParameter.Values);
        parameters.AddRange(whereParams);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private MaterializedStatement MaterializeDelete(Statement statement) {
        var sql = new StringBuilder($"DELETE FROM {statement.GetTableName()}");
        var whereParams = CreateWhereClause(sql, statement.Parameters.WhereParameters);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = whereParams
        };
    }
    
    private MaterializedStatement MaterializeFind(Statement statement) {
        var sql = new StringBuilder($"SELECT {statement.GetSelect()} FROM {statement.GetTableName()}");
        var parameters = CreateWhereClause(sql, statement.Parameters.WhereParameters);
        AddOrderByClause(sql, statement.Parameters.OrderBy);
        if (statement.Parameters.Limit != null) {
            sql.Append($" LIMIT @{LimitParamName}");
            parameters.Add(QueryParameter.From(LimitParamName, statement.Parameters.Limit.Value));
        }
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private MaterializedStatement MaterializeCount(Statement statement) {
        var sql = new StringBuilder($"SELECT COUNT(*) FROM {statement.GetTableName()}");
        var parameters = CreateWhereClause(sql, statement.Parameters.WhereParameters);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }
    
    private List<QueryParameter> CreateWhereClause(StringBuilder sql, WhereParameters multiWhere) {
        return CreateWhereClause(sql, multiWhere.Statements);
    }

    private List<QueryParameter> CreateWhereClause(StringBuilder sql, IEnumerable<QueryParameter> whereStatements) {
        return CreateWhereClause(sql, new[] { new SimpleWhere(whereStatements) });
    }

    private List<QueryParameter> CreateWhereClause(StringBuilder sql, IEnumerable<IWhereStatement> whereStatements) {
        var newParams = new List<QueryParameter>();
        AddWhereClause(sql, newParams, whereStatements);
        return newParams;
    }
    
    private void AddWhereClause(StringBuilder sql, List<QueryParameter> newParams, IEnumerable<IWhereStatement> whereStatements) {
        var first = true;
        foreach (var whereStatement in whereStatements) {
            switch (whereStatement) {
                case SimpleWhere simpleWhere:
                    AddSimpleWhereClause(sql, newParams, simpleWhere.Parameters, ref first);
                    break;
                case RowValueWhere rowValueWhere:
                    AddRowValueWhereClause(sql, newParams, rowValueWhere, ref first);
                    break;
                case SearchWhere searchWhere:
                    AddSearchWhereClause(sql, newParams, searchWhere, ref first);
                    break;
                default:
                    throw new ArgumentException($"Unsupported where clause '{whereStatement.GetType()}'");
            }
        }
    }
    
    private void AddRowValueWhereClause(StringBuilder sql, List<QueryParameter> newParams, RowValueWhere rowValueWhere, ref bool first) {
        if (!rowValueWhere.Parameters.Any()) {
            return;
        }
        AddAndOrWhere(sql, ref first);
        sql.Append(" (");
        sql.Append(string.Join(',', rowValueWhere.Parameters.Select(p => $"{p.Name}")));
        sql.Append(')');
        sql.Append(ComparatorToSqlOperator(rowValueWhere.Comparator));
        sql.Append(" (");
        sql.Append(string.Join(',', rowValueWhere.Parameters.Select(p => $"@{p.ParameterName}")));
        sql.Append(')');
        newParams.AddRange(rowValueWhere.Parameters);
    }

    private void AddSimpleWhereClause(StringBuilder sql, List<QueryParameter> newParams, IEnumerable<QueryParameter> parameters, ref bool first) {
        parameters = PreprocessParameters(parameters);
        foreach (var queryParameter in parameters) {
            if (!AddInClause(sql, newParams, queryParameter, ref first)) {
                AddComparisonClause(sql, newParams, queryParameter, ref first);
            }
        }
    }

    private void AddAndOrWhere(StringBuilder sql, ref bool first) {
        if (first) {
            first = false;
            sql.Append(" WHERE");
        } else {
            sql.Append(" AND");
        }
    }

    private void AddComparisonClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter parameter, ref bool first) {
        AddAndOrWhere(sql, ref first);
        if (parameter.Value == null) {
            sql.Append($" {parameter.Name} IS NULL");
        } else {
            sql.Append($" {parameter.Name} {ComparatorToSqlOperator(parameter.Comparator)} @{parameter.ParameterName}");
            newParams.Add(parameter);
        }
    }

    private bool AddInClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter queryParameter, ref bool first) {
        if (queryParameter.Value is string)
            return false;
        if(queryParameter.Value is not IEnumerable enumerable)
            return false;
        var inParameters = CreateInParameters(queryParameter, enumerable).ToList();
        if (inParameters.Count <= 0) {
            AddAndOrWhere(sql, ref first);
            sql.Append(" 1 != 1");
            return true;
        }

        if (inParameters.Count == 1) {
            queryParameter = inParameters[0];
            AddComparisonClause(sql, newParams, queryParameter, ref first);
            return true;
        }
        AddAndOrWhere(sql, ref first);
        var inParamList = string.Join(',', inParameters.Select(p => $"@{p.ParameterName}"));
        sql.Append($" {queryParameter.Name} IN({inParamList})");
        newParams.AddRange(inParameters);
        return true;
    }

    private void AddSearchWhereClause(StringBuilder sql, List<QueryParameter> newParams, SearchWhere searchWhere, ref bool first) {
        var parameters = searchWhere.Parameters.ToList();
        if (!parameters.Any()) {
            return;
        }
        AddAndOrWhere(sql, ref first);
        if (parameters.Count == 1) {
            AddLikeClause(sql, newParams, parameters[0]);
            return;
        }
        sql.Append(' ').Append('(');
        var innerFirst = true;
        foreach (var queryParameter in parameters) {
            if (queryParameter.Value == null) {
                throw new CaasDbException($"Null parameter {queryParameter.Name} not allowed in search");
            }
            if (innerFirst) {
                innerFirst = false;
            } else {
                sql.Append(" OR ");
            }
            AddLikeClause(sql, newParams, queryParameter);
        }
        sql.Append(')');
    }

    private void AddLikeClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter queryParameter) {
        sql.Append($"LOWER({queryParameter.Name}) LIKE @{queryParameter.ParameterName}");
        newParams.Add(queryParameter with { Value = $"%{queryParameter.Value!.ToString()!.ToLowerInvariant()}%" });
    }

    private void AddOrderByClause(StringBuilder sql, IEnumerable<OrderParameter> parameters) {
        var first = true;
        var firstOrder = true;
        foreach (var parameter in parameters) {
            if (first) {
                sql.Append(" ORDER BY");
                first = false;
            }
            if (firstOrder) {
                firstOrder = false;
            } else {
                sql.Append(',');
            }
            sql.Append($" {parameter.Name} {OrderTypeToSql(parameter.OrderType)}");
        }
    }

    private static string ComparatorToSqlOperator(WhereComparator comparator) {
        switch (comparator) {
            case WhereComparator.Equal:
                return "=";
            case WhereComparator.Greater:
                return ">";
            case WhereComparator.Less:
                return "<";
            case WhereComparator.GreaterOrEqual:
                return ">=";
            case WhereComparator.LessOrEqual:
                return "<=";
            default:
                throw new ArgumentException($"Operator '{comparator}' is invalid.");
        }
    }

    private static string OrderTypeToSql(OrderType orderType) {
        return orderType switch {
            OrderType.Asc => "ASC",
            OrderType.Desc => "DESC",
            _ => throw new ArgumentException()
        };
    }
    

    private static IEnumerable<QueryParameter> CreateInParameters(QueryParameter queryParameter, IEnumerable enumerable) {
        return enumerable.OfType<object>().Select((value, idx) => queryParameter with {
            Value = value, 
            ParameterName = $"{queryParameter.ParameterName}_{idx}"
        });
    }

    private static IEnumerable<QueryParameter> PreprocessParameters(IEnumerable<QueryParameter> parameters) {
        return parameters.Select((queryParameter, parameterIdx) => 
            queryParameter with { ParameterName = $"{queryParameter.ParameterName}_{parameterIdx}" });
    }
}

internal static class StatementExtensions {
    public static string GetTableName(this Statement statement) => $"\"{statement.From}\"" ;
    
    public static string GetSelect(this Statement statement) => GetColumnNamesString(statement.Parameters.SelectParameters.Properties);

    public static string GetInsertColumns(this Statement statement) => GetColumnNamesString(statement.Parameters.InsertParameters.ColumnNames);
    
    public static string GetInsertParameters(this Statement statement) {
        return string.Join(',', statement.Parameters.InsertParameters.Values.Select((values, idx) =>
            "(" + GetColumnNameParametersString(values.Select(v => $"{v.ParameterName}_{idx}")) + ")"));
    }
    
    public static string GetColumnNamesString(IEnumerable<string> columnNames) => string.Join(',', columnNames);
    
    public static string GetColumnNameParametersString(IEnumerable<string> columnNames) => string.Join(',', columnNames.Select(s => $"@{s}"));
}