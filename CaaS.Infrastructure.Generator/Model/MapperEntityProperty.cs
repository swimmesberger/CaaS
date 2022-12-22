using System.Collections.Immutable;

namespace CaaS.Infrastructure.Generator.Model {
    public class MapperEntityProperty {
        public string PropertyName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IsJson { get; set; } = false;
        public IImmutableSet<string>? IgnoredStatementTypes { get; set; }
    }
}