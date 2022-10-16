using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using CaaS.Core.Entities.Base;
using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Repositories.Base;

internal static class Repository {
    public const string ColumnId = "id";
    public const string ColumnRowVersion = "row_version";
    public const string ColumnCreationTime = "creation_time";
    public const string ColumnLastModificationTime = "last_modification_time";
    
    public static readonly string[] ColumnNames = {
            ColumnId, ColumnRowVersion, ColumnCreationTime, ColumnLastModificationTime
    };
}

public abstract class Repository<T> : IRepository<T> where T : Entity, new() {
    private readonly IRelationalConnectionFactory _dbProviderFactory;

    public Repository(IRelationalConnectionFactory dbProviderFactory) {
        _dbProviderFactory = dbProviderFactory;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await GetAllAsyncEnumerable(CancellationToken.None).ToListAsync(cancellationToken);
    }
    
    public Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        await using var dbConnection = _dbProviderFactory.CreateDbConnection();
        await dbConnection.OpenAsync(cancellationToken);
        await using var selCmd = dbConnection.CreateCommand();
        selCmd.CommandText = $"SELECT {string.Join(',', GetColumnNamesImpl())} FROM {GetTableName()} WHERE id = @id";
        selCmd.Parameters.Add(_dbProviderFactory.CreateParameter("id", DbType.Guid));
        selCmd.Connection = dbConnection;
        await using var reader = await selCmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) {
            return default;
        }
        return CreateFromRecord(reader);
    }
    
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    public Task<int> CountAsync(CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<T> GetAllAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken = default) {
        await using var dbConnection = _dbProviderFactory.CreateDbConnection();
        await dbConnection.OpenAsync(cancellationToken);
        await using var selCmd = dbConnection.CreateCommand();
        selCmd.CommandText = $"SELECT {string.Join(',', GetColumnNamesImpl())} FROM {GetTableName()}";
        selCmd.Connection = dbConnection;
        await using var reader = await selCmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) {
            yield return CreateFromRecord(reader);
        }
    }

    private T CreateFromRecord(IDataRecord record) {
        T entity = new T() {
            Id = record.GetGuid(Repository.ColumnId),
            RowVersion = record.GetIn32(Repository.ColumnRowVersion),
            CreationTime = record.GetDateTimeOffset(Repository.ColumnCreationTime),
            LastModificationTime = record.GetDateTimeOffset(Repository.ColumnLastModificationTime)
        };
        entity = SetFromRecord(entity, record);
        return entity;
    }

    private IReadOnlyList<string> GetColumnNamesImpl() {
        List<string> columnNames = new List<string>(Repository.ColumnNames);
        columnNames.AddRange(GetColumnNames());
        return columnNames;
    }

    protected abstract string GetTableName();
    
    protected abstract IReadOnlyList<string> GetColumnNames();

    protected abstract T SetFromRecord(T value, IDataRecord record);
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