using System.Data.Common;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public class AdoStatementGenerator<T> : IStatementGenerator<T> where T: IDataModelBase {
    public IDataRecordMapper<T> DataRecordMapper { get; }
    private readonly IStatementSqlGenerator _materializer;

    public AdoStatementGenerator(IDataRecordMapper<T> recordMapper, IStatementSqlGenerator materializer) {
        DataRecordMapper = recordMapper;
        _materializer = materializer;
    }

    public Statement<long> CreateCount(StatementParameters statementParameters) {
        ValueTask<long> RowMapper(DbDataReader record, CancellationToken token) => record.GetValueAsync<long>(0, token);
        return new Statement<long>(StatementType.Count, _materializer, DataRecordMapper.ByPropertyName(), RowMapper) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = statementParameters
        };
    }

    public Statement<T> CreateFind(StatementParameters statementParameters)
        => CreateFind(statementParameters, DataRecordMapper.EntityFromRecordAsync);

    public Statement<TResult> CreateFind<TResult>(StatementParameters statementParameters, RowMapper<TResult> mapper) {
        if (statementParameters.SelectParameters.IsAll) {
            statementParameters = statementParameters.WithSelect(DataRecordMapper.ByPropertyName().Keys);
        }
        if (statementParameters.SelectParameters.IsEmpty) {
            throw new ArgumentException("Empty select statement");
        }
        return new(StatementType.Find, _materializer, DataRecordMapper.ByPropertyName(), mapper) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = statementParameters
        };
    }

    public Statement<T> CreateInsert(T entity) {
        return CreateInsert(new[] { entity });
    }

    public Statement<T> CreateInsert(IEnumerable<T> entities) {
        var insertValues = entities
                .Select(entity => DataRecordMapper.RecordFromEntity(entity).ByPropertyName())
                .Select(record => GetPropertyNames()
                        .Select(propertyName => QueryParameter.FromTyped(propertyName, record.GetTypedValue(propertyName)))
                        .ToList())
                .ToList();
        if (insertValues.Count == 0) return Statement.CreateEmpty<T>();
        var insertParameters = new InsertParameters() {
            ColumnNames = GetPropertyNames(),
            Values = insertValues
        };
        return new Statement<T>(StatementType.Create, _materializer, DataRecordMapper.ByPropertyName()) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = new StatementParameters() { 
                InsertParameters = insertParameters
            }
        };
    }

    public Statement<T> CreateUpdate(IEnumerable<VersionedEntity<T>> versionedEntities) {
        const string idColumnName = nameof(IDataModelBase.Id);
        const string rowVersionColumnName = nameof(IDataModelBase.RowVersion);
        const string creationColumnName = nameof(IDataModelBase.CreationTime);
        
        var updateParameterList = new List<UpdateParameter>();
        var idx = 0;
        foreach (var versionedEntity in versionedEntities) {
            var record = DataRecordMapper.RecordFromEntity(versionedEntity.Entity).ByPropertyName();

            var updateParameterValues = GetPropertyNames()
                .Where(propertyName => propertyName != idColumnName && propertyName != creationColumnName)
                .Select(propertyName => QueryParameter.FromTyped(propertyName, record.GetTypedValue(propertyName), $"{propertyName}_{idx}"))
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
            ColumnNames = GetPropertyNames(),
            Values = updateParameterList
        };
        return new Statement<T>(StatementType.Update, _materializer, DataRecordMapper.ByPropertyName()) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = new StatementParameters() {
                Update = updateParameters
            }
        };
    }

    public Statement<T> CreateUpdate(T entity, int origRowVersion) => CreateUpdate(new[] { new VersionedEntity<T>(entity, origRowVersion) });

    public Statement<T> CreateDelete(T entity) => CreateDelete(new[] { entity });

    public Statement<T> CreateDelete(IEnumerable<T> entities) {
        return new Statement<T>(StatementType.Delete, _materializer, DataRecordMapper.ByPropertyName()) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = StatementParameters.CreateWhere(QueryParameter.From(
                nameof(IDataModelBase.Id), 
                entities.Select(e => e.Id)
            ))
        };
    }

    private IEnumerable<string> GetPropertyNames() => DataRecordMapper.ByPropertyName().Keys;
}