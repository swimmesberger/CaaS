using System.Data;
using System.Text;
using CaaS.Core.Entities.Base;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories.Base;

public abstract class Repository<T> : IRepository<T> where T : Entity, new() {
    private readonly AdoTemplate _adoTemplate;
    protected IPropertyMapping PropertyMapping { get; }

    public Repository(IConnectionProvider connectionProvider, 
            IPropertyMappingProvider? propertyMappingProvider = null) {
        _adoTemplate = new AdoTemplate(connectionProvider);
        // use the snake_case mapper on default
        propertyMappingProvider ??= ReflectivePropertyMappingProvider<T>.SnakeCaseInstance;
        PropertyMapping = propertyMappingProvider.GetPropertyMapping();
    }

    public async Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default) {
        return await QueryAsync(cancellationToken: CancellationToken.None);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var idColumnName = PropertyMapping.MapProperty(nameof(IEntityBase.Id));
        return (await QueryAsync(
                $" WHERE {idColumnName} = @{idColumnName}",
                new[]{new QueryParameter(idColumnName, id)}, 
                cancellationToken)).FirstOrDefault();
    }
    
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var origRowVersion = entity.RowVersion;
        entity = entity with {
            RowVersion = origRowVersion+1,
            LastModificationTime = DateTimeOffset.UtcNow
        };
        var statement = CreateUpdateStatement(entity, origRowVersion);
        var updatedEntries = await _adoTemplate.ExecuteAsync(statement, cancellationToken);
        if (updatedEntries == 0) {
            throw new DbUpdateConcurrencyException();
        }
        return entity;
    }
    
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public Task<int> CountAsync(CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

    protected Task<List<T>> QueryAsync(string? sqlSuffix = null, 
            IEnumerable<QueryParameter>? parameters = null,
            CancellationToken cancellationToken = default) {
        var sql = $"SELECT {string.Join(',', GetColumnNames())} FROM {GetTableName()}{sqlSuffix}";
        return _adoTemplate.QueryAsync(
                new Statement(sql, parameters), 
                CreateFromRecord,
                cancellationToken: cancellationToken);
    }
    
    private T CreateFromRecord(IDataRecord record) {
        var entity = new T();
        entity = SetFromRecord(entity, record);
        return entity;
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

    private IEnumerable<string> GetColumnNames() => PropertyMapping.Columns;
    
    protected abstract string GetTableName();

    protected virtual T SetFromRecord(T value, IDataRecord record) {
        return value with {
            Id = record.GetGuid(PropertyMapping.MapProperty(nameof(IEntityBase.Id))),
            RowVersion = record.GetIn32(PropertyMapping.MapProperty(nameof(IEntityBase.RowVersion))),
            CreationTime = record.GetDateTimeOffset(PropertyMapping.MapProperty(nameof(IEntityBase.CreationTime))),
            LastModificationTime = record.GetDateTimeOffset(PropertyMapping.MapProperty(nameof(IEntityBase.LastModificationTime)))
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

internal static class AsyncEnumerableExtensions {
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken cancellationToken = default) {
        var list = new List<T>();
        await foreach (var value in asyncEnumerable.WithCancellation(cancellationToken)) {
            list.Add(value);
        }
        return list;
    }
}