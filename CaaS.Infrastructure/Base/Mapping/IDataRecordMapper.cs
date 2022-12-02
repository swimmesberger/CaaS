using System.Data.Common;
using CaaS.Infrastructure.Base.Ado;

namespace CaaS.Infrastructure.Base.Mapping; 

public interface IDataRecordMapper<T> : IPropertyMapping, IHasMetadataProvider {
    ValueTask<T> EntityFromRecordAsync(DbDataReader record, CancellationToken cancellationToken = default);

    IRecord RecordFromEntity(T record);
}