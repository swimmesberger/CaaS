using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IDataRecordProvider<T> {
    IDataRecordMapper<T> DataRecordMapper { get; }
}