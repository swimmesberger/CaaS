using System.Data.Common;

namespace CaaS.Infrastructure.Base.Mapping; 

public interface IDataRecordMapper<T> : IPropertyMapping {
    string MappedTypeName { get; }

    IRecordMetadataProvider MetadataProvider { get; }

    ValueTask<T> EntityFromRecordAsync(DbDataReader record, CancellationToken cancellationToken = default);

    IRecord RecordFromEntity(T record);
}