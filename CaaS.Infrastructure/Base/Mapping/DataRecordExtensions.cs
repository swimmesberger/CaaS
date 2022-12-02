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
        
        return new ValueTask<T>(record.GetFieldValue<T>(ordinal));
    }
}