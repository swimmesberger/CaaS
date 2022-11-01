using System.Data;

namespace CaaS.Infrastructure.DataMapping; 

public interface IDataRecordMapper<T> : IPropertyMapping {
    string MappedTypeName { get; }

    T EntityFromRecord(IDataRecord record);

    IRecord RecordFromEntity(T record);
}