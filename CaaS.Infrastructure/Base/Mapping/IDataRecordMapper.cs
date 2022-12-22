using System.Data.Common;
using CaaS.Infrastructure.Base.Ado;

namespace CaaS.Infrastructure.Base.Mapping; 

public interface IDataRecordMapper<T> : IPropertyMapping, IHasMetadataProvider {
    IReadOnlyList<PropertyMetadata> Properties { get; }
    
    ValueTask<T> EntityFromRecordAsync(DbDataReader record, CancellationToken cancellationToken = default);

    IRecord RecordFromEntity(T record);
}