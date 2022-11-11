using System.Data.Common;

namespace CaaS.Infrastructure.Base.DataMapping; 

public interface IDataRecordMapper<T> : IPropertyMapping {
    string MappedTypeName { get; }

    ValueTask<T> EntityFromRecordAsync(DbDataReader record, CancellationToken cancellationToken = default);

    IRecord RecordFromEntity(T record);
}