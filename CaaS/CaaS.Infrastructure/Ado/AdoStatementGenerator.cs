using System.Collections;
using System.Text;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping;
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
        var record = DataRecordMapper.RecordFromEntity(entity).ByColumName();
        var parameters = GetColumnNames()
                .Select(columnName => new QueryParameter(columnName, record.GetObject(columnName)))
                .ToList();
        return new Statement(StatementType.Create, this) {
            Parameters = new StatementParameters() { 
                Other = parameters
            }
        };
    }

    public Statement CreateUpdate(T entity, int origRowVersion) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        var idColumnName = propertyMapper.MapName(nameof(IDataModelBase.Id));
        var rowVersionColumnName = propertyMapper.MapName(nameof(IDataModelBase.RowVersion));
        var creationColumnName = propertyMapper.MapName(nameof(IDataModelBase.CreationTime));
        var record = DataRecordMapper.RecordFromEntity(entity).ByColumName();

        var updateParameters = GetColumnNames()
                .Where(columnName => columnName != idColumnName && columnName != creationColumnName)
                .Select(columnName => new QueryParameter(columnName, record.GetObject(columnName)))
                .ToList();
        var whereParameters = new List<QueryParameter>() {
                new(idColumnName, entity.Id),
                new(rowVersionColumnName, origRowVersion) { ParameterName = "curRowVersion" }
        };
        return new Statement(StatementType.Update, this) {
            Parameters = new StatementParameters() {
                Where = whereParameters,
                Other = updateParameters
            }
        };
    }

    public Statement CreateDelete(T entity) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        return new Statement(StatementType.Delete, this) {
            Parameters = new StatementParameters() {
                Where = new List<QueryParameter>(){ new(propertyMapper.MapName(nameof(IDataModelBase.Id)), entity.Id) }
            }
        };
    }

    public MaterializedStatement MaterializeStatement(Statement statement) {
        return statement.Type switch {
            StatementType.Count => new MaterializedStatement($"SELECT COUNT(*) FROM {GetTableName()}"),
            StatementType.Create => new MaterializedStatement(
                    $"INSERT INTO {GetTableName()} ({GetColumnNamesString()}) VALUES ({GetColumnNameParametersString()})") {
                    Parameters = statement.Parameters.BindParameters
            },
            StatementType.Update => MaterializeUpdate(statement),
            StatementType.Delete => MaterializeDelete(statement),
            StatementType.Find => MaterializeFind(statement),
            _ => throw new ArgumentException()
        };
    }

    private MaterializedStatement MaterializeUpdate(Statement statement) {
        var sql = new StringBuilder("UPDATE");
        sql.Append(' ').Append(GetTableName());
        sql.Append(" SET");
        var first = true;
        foreach (var parameter in statement.Parameters.Other) {
            if (first) {
                first = false;
            } else {
                sql.Append(',');
            }
            sql.Append(' ').Append(parameter.Name).Append(" = ").Append('@').Append(parameter.ParameterName);
        }
        var whereParams = CreateWhereClause(sql, statement.Parameters.Where);
        return new MaterializedStatement(sql.ToString()) {
            Parameters = statement.Parameters.Other.Concat(whereParams).ToList()
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
            if (first) {
                first = false;
                sql.Append(" WHERE");
            } else {
                sql.Append(" AND");
            }
            var explodedParams = ExplodeParameters(queryParameter).ToList();
            if (explodedParams.Count > 1) {
                var inParamList = string.Join(',', explodedParams.Select(p => $"@{p.ParameterName}"));
                sql.Append($" {queryParameter.Name} IN({inParamList})");
            } else {
                sql.Append($" {queryParameter.Name} = @{explodedParams[0].ParameterName}");
            }
            newParams.AddRange(explodedParams);
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

    private string GetColumnNamesString() => string.Join(',', GetColumnNames());
    
    private string GetColumnNameParametersString() => string.Join(',', GetColumnNames().Select(s => $"@{s}"));

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