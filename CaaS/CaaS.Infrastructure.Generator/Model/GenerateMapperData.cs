using System;
using CaaS.Infrastructure.Generator.Mapping;

namespace CaaS.Infrastructure.Generator.Model {
    public class GenerateMapperData {
        public GenerateMapperEntity[] EntityTypes { get; set; } = Array.Empty<GenerateMapperEntity>();
        public PropertyNamingPolicy NamingPolicy { get; set; } = PropertyNamingPolicy.SnakeCase;
    }
}