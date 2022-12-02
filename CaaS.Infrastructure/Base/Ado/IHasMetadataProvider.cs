using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IHasMetadataProvider {
    IRecordMetadataProvider MetadataProvider { get; }
}