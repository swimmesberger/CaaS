using System.Collections;
using System.Text;
using CaaS.Core.Base.Exceptions;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public class AdoStatementGenerator<T> : IStatementGenerator<T>, IStatementSqlGenerator where T: IDataModelBase {
    public IDataRecordMapper<T> DataRecordMapper { get; }

    public AdoStatementGenerator(IDataRecordMapper<T> recordMapper) {
        DataRecordMapper = recordMapper;
    }

    public Statement CreateCount(StatementParameters statementParameters) 
        => new Statement(StatementType.Count, this) {
            Parameters = statementParameters
        };

    public Statement CreateFind(StatementParameters statementParameters)
        => new Statement(StatementType.Find, this) {
            Parameters = statementParameters
        };

    public Statement CreateInsert(T entity) {
        return CreateInsert(new[] { entity });
    }

    public Statement CreateInsert(IEnumerable<T> entities) {
        var insertValues = entities
                .Select(entity => DataRecordMapper.RecordFromEntity(entity).ByColumName())
                .Select(record => GetColumnNames()
                        .Select(columnName => QueryParameter.FromTyped(columnName, record.GetTypedValue(columnName)))
                        .ToList())
                .ToList();
        if (insertValues.Count == 0) return Statement.Empty;
        var insertParameters = new InsertParameters() {
            ColumnNames = DataRecordMapper.ByColumName().Keys,
            Values = insertValues
        };
        return new Statement(StatementType.Create, this) {
            Parameters = new StatementParameters() { 
                Insert = insertParameters
            }
        };
    }

    public Statement CreateUpdate(IEnumerable<VersionedEntity<T>> versionedEntities) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        var idColumnName = propertyMapper.MapName(nameof(IDataModelBase.Id));
        var rowVersionColumnName = propertyMapper.MapName(nameof(IDataModelBase.RowVersion));
        var creationColumnName = propertyMapper.MapName(nameof(IDataModelBase.CreationTime));
        
        var updateParameterList = new List<UpdateParameter>();
        var idx = 0;
        foreach (var versionedEntity in versionedEntities) {
            var record = DataRecordMapper.RecordFromEntity(versionedEntity.Entity).ByColumName();

            var updateParameterValues = GetColumnNames()
                .Where(columnName => columnName != idColumnName && columnName != creationColumnName)
                .Select(columnName => QueryParameter.FromTyped(columnName, record.GetTypedValue(columnName), $"{columnName}_{idx}"))
                .ToList();
            var whereParameters = new List<QueryParameter>() {
                QueryParameter.From(idColumnName, versionedEntity.Entity.Id),
                QueryParameter.From(rowVersionColumnName, versionedEntity.RowVersion, "curRowVersion")
            };
            updateParameterList.Add(new UpdateParameter() {
                Values = updateParameterValues,
                Where = whereParameters
            });
            idx += 1;
        }
        var updateParameters = new UpdateParameters() {
            ColumnNames = GetColumnNames(),
            Values = updateParameterList
        };
        return new Statement(StatementType.Update, this) {
            Parameters = new StatementParameters() {
                Update = updateParameters
            }
        };
    }

    public Statement CreateUpdate(T entity, int origRowVersion) => CreateUpdate(new[] { new VersionedEntity<T>(entity, origRowVersion) });

    public Statement CreateDelete(T entity) => CreateDelete(new[] { entity });

    public Statement CreateDelete(IEnumerable<T> entities) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        
        return new Statement(StatementType.Delete, this) {
            Parameters = StatementParameters.CreateWhere(QueryParameter.From(
                propertyMapper.MapName(nameof(IDataModelBase.Id)), 
                entities.Select(e => e.Id)
            ))
        };
    }

    public MaterializedStatements MaterializeStatement(Statement statement) {
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
        var insertValues = string.Join(',', statement.Parameters.Insert.Values.Select((values, idx) => 
                "(" + GetColumnNameParametersString(values.Select(v => $"{v.ParameterName}_{idx}")) + ")"));
        var sql = $"INSERT INTO {GetTableName()} ({GetColumnNamesString(statement.Parameters.Insert.ColumnNames)}) VALUES {insertValues}";
        var parameters = statement.Parameters.Insert.Values.SelectMany((values, idx) => {
            return values.Select(q => q with { ParameterName = $"{q.ParameterName}_{idx}" });
        });
        return new MaterializedStatement(sql) {
            Parameters = parameters
        };
    }

    private MaterializedStatements MaterializeUpdate(Statement statement) {
        return new MaterializedStatements(statement.Parameters.Update.Values.Select(MaterializeUpdate).ToList());
    }

    private MaterializedStatement MaterializeUpdate(UpdateParameter updateParameter) {
        var sql = new StringBuilder("UPDATE");
        sql.Append(' ').Append(GetTableName());
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
        var sql = new StringBuilder($"DELETE FROM {GetTableName()}");
        var whereParams = CreateWhereClause(sql, statement.Parameters.Where);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = whereParams
        };
    }
    
    private MaterializedStatement MaterializeFind(Statement statement) {
        var sql = new StringBuilder($"SELECT {GetColumnNamesString()} FROM {GetTableName()}");
        var parameters = CreateWhereClause(sql, statement.Parameters.Where);
        AddOrderByClause(sql, statement.Parameters.OrderBy);
        if (statement.Parameters.Limit != null) {
            sql.Append($" LIMIT @{QueryParameter.LimitParamName}");
            parameters.Add(QueryParameter.From(QueryParameter.LimitParamName, statement.Parameters.Limit.Value));
        }
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private MaterializedStatement MaterializeCount(Statement statement) {
        var sql = new StringBuilder($"SELECT COUNT(*) FROM {GetTableName()}");
        var parameters = CreateWhereClause(sql, statement.Parameters.Where);
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
        if (inParameters.Count <= 0) 
            return true;
        
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

    private string ComparatorToSqlOperator(WhereComparator comparator) {
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
            sql.Append($" {parameter.Name} {OrderTypeSql(parameter.OrderType)}");
        }
    }

    private string OrderTypeSql(OrderType orderType) {
        return orderType switch {
            OrderType.Asc => "ASC",
            OrderType.Desc => "DESC",
            _ => throw new ArgumentException()
        };
    }

    private string GetColumnNamesString() => GetColumnNamesString(GetColumnNames());
    
    private string GetColumnNamesString(IEnumerable<string> columnNames) => string.Join(',', columnNames);

    private string GetColumnNameParametersString(IEnumerable<string> columnNames) => string.Join(',', columnNames.Select(s => $"@{s}"));

    private IEnumerable<string> GetColumnNames() => DataRecordMapper.ByColumName().Keys;

    private string GetTableName() => $"\"{DataRecordMapper.MappedTypeName}\"" ;
    
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