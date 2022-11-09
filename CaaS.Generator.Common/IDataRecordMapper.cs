using System.Data;

namespace CaaS.Generator.Common {
    public interface IDataRecordMapper<T> : IPropertyMapping {
        string MappedTypeName { get; }

        T EntityFromRecord(IDataRecord record);

        IRecord RecordFromEntity(T record);
    }
}