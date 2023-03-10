using System.Data.Common;

namespace CaaS.Infrastructure.Base.Mapping; 

public static class DataRecordExtensions {
    // ReSharper disable once UnusedParameter.Global
    public static ValueTask<T> GetValueAsync<T>(this DbDataReader record, string key, CancellationToken cancellationToken = default) {
        // Possibility for async access when Command is created with CommandBehavior.SequentialAccess
        var ordinal = record.GetOrdinal(key);
        return record.GetValueAsync<T>(ordinal, cancellationToken);
    }
    
    // ReSharper disable once UnusedParameter.Global
    public static ValueTask<T> GetValueAsync<T>(this DbDataReader record, int ordinal, CancellationToken cancellationToken = default) {
        // Guid? leads to InvalidCastException
        if (record.IsDBNull(ordinal)) {
            return default;
        }
        if (typeof(T) == typeof(ReadOnlyMemory<byte>)) {
            ReadOnlyMemory<byte> data = record.GetFieldValue<byte[]>(ordinal);
            return new ValueTask<T>((T)(object)data);
        }
        if (typeof(T) == typeof(Memory<byte>)) {
            Memory<byte> data = record.GetFieldValue<byte[]>(ordinal);
            return new ValueTask<T>((T)(object)data);
        }
        return new ValueTask<T>(record.GetFieldValue<T>(ordinal));
    }
}