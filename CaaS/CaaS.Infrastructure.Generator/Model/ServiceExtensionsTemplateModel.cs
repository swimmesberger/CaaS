using System.Collections.Generic;
using System.Linq;

namespace CaaS.Infrastructure.Generator.Model {
    internal class ServiceExtensionsTemplateModel {
        public string Namespace { get; set; } = InitializationContext.GenerateNamespace;
        public string Version { get; set; } = InitializationContext.Version;
        public List<RowMapper> Mappers { get; } = new List<RowMapper>();
        public IEnumerable<string> UsingNamespaces => Mappers.Select(m => m.EntityNamespace).Distinct();
    }
}