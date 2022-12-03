using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public class EmptyPropertyMapping : IPropertyMapping {
    public static readonly IPropertyMapping Empty = new EmptyPropertyMapping();
    public static readonly IPropertyMapper EmptyMapper = new EmptyPropertyMapper();

    public IPropertyMapper ByColumName() => EmptyMapper;

    public IPropertyMapper ByPropertyName() => EmptyMapper;

    private class EmptyPropertyMapper : IPropertyMapper {
        public string TypeName => string.Empty;
        public string MappedTypeName => string.Empty;
        
        public IEnumerable<string> Keys => Enumerable.Empty<string>();

        public string MapName(string key) => key;
    }
}