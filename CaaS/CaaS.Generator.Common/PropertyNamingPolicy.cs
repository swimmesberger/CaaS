namespace CaaS.Generator.Common {
    public abstract class PropertyNamingPolicy {
        public static PropertyNamingPolicy SnakeCase { get; } = new PropertySnakeCaseNamingPolicy();

        public abstract string ConvertName(string name);
    }
}