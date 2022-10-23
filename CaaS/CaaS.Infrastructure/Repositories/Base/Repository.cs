using System.Data;
using System.Text;
using CaaS.Core.Entities.Base;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories.Base;

public abstract class Repository<T> : IRepository<T> where T : Entity, new() {
    private readonly IQueryExecutor _queryExecutor;
    protected IPropertyMapping PropertyMapping { get; }

    public Repository(IQueryExecutor queryExecutor, IPropertyMappingProvider<T> propertyMappingProvider) {
        _queryExecutor = queryExecutor;
        PropertyMapping = propertyMappingProvider.GetPropertyMapping();
    }

    public async Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default) {
        return await QueryAsync(cancellationToken: CancellationToken.None);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var idColumnName = PropertyMapping.MapProperty(nameof(IEntityBase.Id));
        return (await QueryAsync(
                $" AND {idColumnName} = @{idColumnName}",
                new[]{new QueryParameter(idColumnName, id)}, 
                cancellationToken)).FirstOrDefault();
    }
    
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await _queryExecutor.ExecuteAsync(CreateInsertStatement(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
        return entity;
    }
    
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var origRowVersion = entity.RowVersion;
        entity = entity with {
            RowVersion = origRowVersion+1,
            LastModificationTime = DateTimeOffset.UtcNow
        };
        var statement = CreateUpdateStatement(entity, origRowVersion);
        var changedCount = await _queryExecutor.ExecuteAsync(statement, cancellationToken);
        if (changedCount == 0) {
            throw new CaasUpdateConcurrencyDbException();
        }
        return entity;
    }
    
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var sql = $"DELETE FROM {GetTableName()} WHERE {PropertyMapping.MapProperty(nameof(IEntityBase.Id))} = @id";
        var changedCount = await _queryExecutor.ExecuteAsync(
                new Statement(sql, new[] { new QueryParameter("id", entity.Id) }), 
                cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
    }
    
    public async Task<long> CountAsync(CancellationToken cancellationToken = default) {
        var sql = $"SELECT COUNT(*) FROM {GetTableName()}";
        return (long)(await _queryExecutor
                .QueryScalarAsync(new Statement(sql), cancellationToken) ?? throw new InvalidOperationException());
    }

    protected async Task<List<T>> QueryAsync(string? sqlSuffix = null, 
            IEnumerable<QueryParameter>? parameters = null,
            CancellationToken cancellationToken = default) {
        var sql = $"SELECT {GetColumnNamesString()} FROM {GetTableName()} WHERE 1=1{sqlSuffix}";
        var statement = new Statement(sql, parameters);
        statement = await PostProcessStatement(statement, cancellationToken);
        return await _queryExecutor.QueryAsync(
                statement, 
                CreateFromRecord,
                cancellationToken: cancellationToken);
    }

    protected virtual Task<Statement> PostProcessStatement(Statement statement, 
            CancellationToken cancellationToken = default) => Task.FromResult(statement);

    private T CreateFromRecord(IDataRecord record) => CreateFromRecord(new RecordValues(record, PropertyMapping));

    private T CreateFromRecord(RecordValues record) {
        var entity = new T();
        entity = SetFromRecord(entity, record);
        return entity;
    }

    private Statement CreateInsertStatement(T entity) {
        var sb = new StringBuilder("INSERT INTO");
        sb.Append(' ').Append(GetTableName());
        sb.Append('(').Append(GetColumnNamesString()).Append(')');
        sb.Append("VALUES");
        sb.Append(' ').Append('(').Append(string.Join(',', GetColumnNames()
                .Select(s => $"@{s}"))).Append(')');
        var parameters = GetColumnNames()
                .Select(columnName => new QueryParameter(
                        columnName, 
                        GetRecordValue(entity, PropertyMapping.MapColumn(columnName))
                )).ToList();
        return new Statement(sb.ToString(), parameters);
    }

    private Statement CreateUpdateStatement(T entity, int origRowVersion) {
        var idColumnName = PropertyMapping.MapProperty(nameof(IEntityBase.Id));
        var rowVersionColumnName = PropertyMapping.MapProperty(nameof(IEntityBase.RowVersion));
        var creationColumnName = PropertyMapping.MapProperty(nameof(IEntityBase.CreationTime));

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
            parameters.Add(new QueryParameter(columnName, 
                    GetRecordValue(entity, PropertyMapping.MapColumn(columnName))));
        }
        sb.Append(" WHERE ").Append(idColumnName).Append(" = ").Append('@').Append(idColumnName);
        sb.Append(" AND ").Append(rowVersionColumnName).Append(" = ").Append("@curRowVersion").Append("");
        return new Statement(sb.ToString(), parameters);
    }

    private string GetColumnNamesString() => string.Join(',', GetColumnNames());

    private IEnumerable<string> GetColumnNames() => PropertyMapping.Columns;
    
    protected abstract string GetTableName();

    protected virtual T SetFromRecord(T value, RecordValues record) {
        return value with {
            Id = record.GetGuid(nameof(IEntityBase.Id)),
            RowVersion = record.GetIn32(nameof(IEntityBase.RowVersion)),
            CreationTime = record.GetDateTimeOffset(nameof(IEntityBase.CreationTime)),
            LastModificationTime = record.GetDateTimeOffset(nameof(IEntityBase.LastModificationTime))
        };
    }

    protected virtual object GetRecordValue(T value, string propertyName) {
        return propertyName switch {
                nameof(IEntityBase.Id) => value.Id,
                nameof(IEntityBase.RowVersion) => value.RowVersion,
                nameof(IEntityBase.CreationTime) => value.CreationTime,
                nameof(IEntityBase.LastModificationTime) => value.LastModificationTime,
                _ => throw new ArgumentException("Unsupported property", nameof(propertyName))
        };
    }
}