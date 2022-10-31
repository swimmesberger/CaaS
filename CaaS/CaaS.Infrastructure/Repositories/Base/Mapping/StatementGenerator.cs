using System.Text;
using CaaS.Core.Entities.Base;
using CaaS.Generator.Common;
using CaaS.Infrastructure.Ado;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public class StatementGenerator<T> : IStatementGenerator<T> where T: IEntityBase {
    public IDataRecordMapper<T> DataRecordMapper { get; }

    public StatementGenerator(IDataRecordMapper<T> recordMapper) {
        DataRecordMapper = recordMapper;
    }

    public Statement CreateCount() {
        var sql = $"SELECT COUNT(*) FROM {GetTableName()}";
        return new Statement(sql);
    }

    public Statement CreateFind(IEnumerable<QueryParameter>? parameters = null) {
        var sql = new StringBuilder($"SELECT {GetColumnNamesString()} FROM {GetTableName()} WHERE 1=1");
        // ReSharper disable once PossibleMultipleEnumeration
        var statement = new Statement(sql.ToString(), parameters);
        if (parameters != null) {
            // ReSharper disable once PossibleMultipleEnumeration
            statement = AddFindParameters(statement, parameters);
        }
        return statement;
    }

    public Statement AddFindParameters(Statement statement, IEnumerable<QueryParameter> parameters) {
        var sql = new StringBuilder(statement.Sql);
        // ReSharper disable once PossibleMultipleEnumeration
        foreach (var queryParameter in parameters) {
            sql.Append($" AND {queryParameter.Name} = @{queryParameter.Name}");
        }
        // ReSharper disable once PossibleMultipleEnumeration
        return new Statement(sql.ToString(), parameters);
    }

    public Statement AddFindParameter(Statement statement, QueryParameter queryParameter)
        => AddFindParameters(statement, new[] { queryParameter });

    public Statement AddFindParameterByProperty(Statement statement, string propertyName, object value)
        => AddFindParameter(statement, new QueryParameter(DataRecordMapper.ByPropertyName().MapName(propertyName), value));

    public Statement CreateInsert(T entity) {
        var record = DataRecordMapper.RecordFromEntity(entity).ByColumName();
        
        var sb = new StringBuilder("INSERT INTO");
        sb.Append(' ').Append(GetTableName());
        sb.Append('(').Append(GetColumnNamesString()).Append(')');
        sb.Append("VALUES");
        sb.Append(' ').Append('(').Append(string.Join(',', GetColumnNames()
                .Select(s => $"@{s}"))).Append(')');
        var parameters = GetColumnNames()
                .Select(columnName => new QueryParameter(
                        columnName, 
                        record.GetObject(columnName)
                )).ToList();
        return new Statement(sb.ToString(), parameters);
    }

    public Statement CreateUpdate(T entity, int origRowVersion) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        var idColumnName = propertyMapper.MapName(nameof(IEntityBase.Id));
        var rowVersionColumnName = propertyMapper.MapName(nameof(IEntityBase.RowVersion));
        var creationColumnName = propertyMapper.MapName(nameof(IEntityBase.CreationTime));
        var record = DataRecordMapper.RecordFromEntity(entity).ByColumName();
        
        var sb = new StringBuilder("UPDATE");
        sb.Append(' ').Append(GetTableName());
        sb.Append(" SET ");
        var parameters = new List<QueryParameter> {
                new(idColumnName, entity.Id),
                new("curRowVersion", origRowVersion)
        };
        var first = true;
        foreach (var columnName in GetColumnNames()) {
            if(columnName == idColumnName ||
               columnName == creationColumnName) continue;

            if (first) {
                first = false;
            } else {
                sb.Append(", ");
            }
            sb.Append(' ').Append(columnName).Append(" = ").Append('@').Append(columnName);
            parameters.Add(new QueryParameter(columnName, record.GetObject(columnName)));
        }
        sb.Append(" WHERE ").Append(idColumnName).Append(" = ").Append('@').Append(idColumnName);
        sb.Append(" AND ").Append(rowVersionColumnName).Append(" = ").Append("@curRowVersion").Append("");
        return new Statement(sb.ToString(), parameters);
    }

    public Statement CreateDelete(T entity) {
        var propertyMapper = DataRecordMapper.ByPropertyName();
        var sql = $"DELETE FROM {GetTableName()} WHERE {propertyMapper.MapName(nameof(IEntityBase.Id))} = @id";
        return new Statement(sql, new[] { new QueryParameter("id", entity.Id) });
    }

    private string GetColumnNamesString() => string.Join(',', GetColumnNames());

    private IEnumerable<string> GetColumnNames() => DataRecordMapper.ByColumName().Keys;

    private string GetTableName() => DataRecordMapper.MappedTypeName;
}