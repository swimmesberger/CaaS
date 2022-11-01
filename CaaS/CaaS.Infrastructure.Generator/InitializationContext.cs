using Scriban;

namespace CaaS.Infrastructure.Generator {
    public class InitializationContext {
        public const string GenerateMapperAttributeName = "CaaS.Infrastructure.DataMapping.GenerateMapper";
        public const string GenerateMapperSimpleAttributeName = "GenerateMapper";
        public const string GenerateNamespace = "CaaS.Infrastructure.Gen";
        public const string TenantIdColumnAttributeName = "CaaS.Infrastructure.DataMapping.TenantIdColumn";
        public const string EntitySuffix = "DataModel";
        public static readonly string Version = typeof(InitializationContext).Assembly.GetName().Version.ToString();

        public Template DataMapperTemplate { get; }
        public Template DataMapperServiceExtensionsTemplate { get; }

        public InitializationContext(Template dataMapperTemplate, Template dataMapperServiceExtensionsTemplate) {
            DataMapperTemplate = dataMapperTemplate;
            DataMapperServiceExtensionsTemplate = dataMapperServiceExtensionsTemplate;
        }
    }
}