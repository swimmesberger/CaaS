using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace CaaS.Generator.Common {
    public class GenerateMapper : Attribute {
        public Type[] EntityTypes { get; }
        public Type? NamingPolicyType { get; }

        public GenerateMapper(Type entityType, Type? namingPolicyType = null) : this(new[] { entityType }, namingPolicyType) { }

        public GenerateMapper(Type[] entityTypes, Type? namingPolicyType = null) {
            EntityTypes = entityTypes;
            NamingPolicyType = namingPolicyType;
        }
    }
}