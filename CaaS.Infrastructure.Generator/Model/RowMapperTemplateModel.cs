namespace CaaS.Infrastructure.Generator.Model {
    public class RowMapperTemplateModel {
        public string Namespace { get; set; } = InitializationContext.GenerateNamespace;
        public string Version { get; set; } = InitializationContext.Version;
        public GenerateMapperEntity Entity { get; set; } = new GenerateMapperEntity();
    }
}