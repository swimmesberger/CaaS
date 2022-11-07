using System.Collections;
using System.Text;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping.Base;
using CaaS.Infrastructure.DataModel.Base;

namespace CaaS.Infrastructure.Ado; 

public class AdoStatementGenerator<T> : IStatementGenerator<T>, IStatementSqlGenerator where T: IDataModelBase {
    public IDataRecordMapper<T> DataRecordMapper { get; }

    public AdoStatementGenerator(IDataRecordMapper<T> recordMapper) {
        DataRecordMapper = recordMapper;
    }

    public Statement CreateCount() => new Statement(StatementType.Count, this);

    public Statement CreateFind(StatementParameters statementParameters) {
        // ReSharper disable once PossibleMultipleEnumeration
        return new Statement(StatementType.Find, this) {
            Parameters = statementParameters
        };
    }

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
                QueryParameter.From(rowVersionColumnName, versionedEntity.RowVersion, $"curRowVersion_{idx}")
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

    public Statement CreateDelete(T entity) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        return new Statement(StatementType.Delete, this) {
            Parameters = new StatementParameters() {
                Where = new List<QueryParameter>(){ QueryParameter.From(propertyMapper.MapName(nameof(IDataModelBase.Id)), entity.Id) }
            }
        };
    }

    public MaterializedStatement MaterializeStatement(Statement statement) {
        return statement.Type switch {
            StatementType.Count => new MaterializedStatement($"SELECT COUNT(*) FROM {GetTableName()}"),
            StatementType.Create => MaterializeInsert(statement),
            StatementType.Update => MaterializeUpdate(statement),
            StatementType.Delete => MaterializeDelete(statement),
            StatementType.Find => MaterializeFind(statement),
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

    private MaterializedStatement MaterializeUpdate(Statement statement) {
        var sql = new StringBuilder();
        var parameters = new List<QueryParameter>();
        var first = true;
        foreach (var updateParameter in statement.Parameters.Update.Values) {
            if (first) {
                first = false;
            } else {
                sql.Append("; ");
            }
            MaterializeUpdate(updateParameter, sql, parameters);
        }
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private void MaterializeUpdate(UpdateParameter updateParameter, StringBuilder sql, List<QueryParameter> parameters) {
        sql.Append("UPDATE");
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
        var whereParams = CreateWhereClause(sql, updateParameter.Where);
        parameters.AddRange(updateParameter.Values);
        parameters.AddRange(whereParams);
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
        var whereParams = CreateWhereClause(sql, statement.Parameters.Where);
        AddOrderByClause(sql, statement.Parameters.OrderBy);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = whereParams
        };
    }
    private IEnumerable<QueryParameter> CreateWhereClause(StringBuilder sql, IEnumerable<QueryParameter> parameters) {
        var newParams = new List<QueryParameter>();
        AddWhereClause(sql, newParams, parameters);
        return newParams;
    }

    private void AddWhereClause(StringBuilder sql, List<QueryParameter> newParams, IEnumerable<QueryParameter> parameters) {
        var first = true;
        foreach (var queryParameter in parameters) {
            var explodedParams = ExplodeParameters(queryParameter).ToList();
            if (explodedParams.Count <= 0) continue;
            if (first) {
                first = false;
                sql.Append(" WHERE");
            } else {
                sql.Append(" AND");
            }
            if (explodedParams.Count > 1) {
                var inParamList = string.Join(',', explodedParams.Select(p => $"@{p.ParameterName}"));
                sql.Append($" {queryParameter.Name} IN({inParamList})");
                newParams.AddRange(explodedParams);
            } else {
                var parameter = explodedParams[0];
                if (parameter.Value == null) {
                    sql.Append($" {queryParameter.Name} IS NULL");
                } else {
                    sql.Append($" {queryParameter.Name} = @{parameter.ParameterName}");
                    newParams.Add(parameter);
                }
            }
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
    
    private static IEnumerable<QueryParameter> ExplodeParameters(QueryParameter queryParameter) {
        if (queryParameter.Value is IEnumerable enumerable and not string) {
            return enumerable.OfType<object>().Select((value, idx) => queryParameter with {
                    Value = value, 
                    ParameterName = $"{queryParameter.Name}_{idx}"
            });
        }
        return new[] { queryParameter };
    }
}