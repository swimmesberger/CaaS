using System.Data;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public interface IRecordMapper<T> : IPropertyMapping {
    string MappedTypeName { get; }
    
    T EntityFromRecord(IDataRecord record);

    IRecord RecordFromEntity(T record);
}