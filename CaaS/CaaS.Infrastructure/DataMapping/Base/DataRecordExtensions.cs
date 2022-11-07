using System.Data;
using System.Data.Common;

namespace CaaS.Infrastructure.DataMapping.Base; 

public static class DataRecordExtensions {
    // ReSharper disable once UnusedParameter.Global
    public static ValueTask<T> GetValueAsync<T>(this DbDataReader record, string key, CancellationToken cancellationToken = default) {
        // Possibility for async access when Command is created with CommandBehavior.SequentialAccess
        return new ValueTask<T>(record.GetFieldValue<T>(key));
    }
}