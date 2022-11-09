using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaaS.Infrastructure.Generator.Mapping;
using CaaS.Infrastructure.Generator.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;

namespace CaaS.Infrastructure.Generator {
    public class SyntaxReceiver : ISyntaxContextReceiver {

        
        public InitializationContext InitializationContext { get; }
        public List<GenerateMapperData> GenerateMapperData { get; } = new List<GenerateMapperData>();

        public SyntaxReceiver() {
            var dataMapperTemplate = LoadTemplate("DataRecordMapper.scriban-cs");
            var dataMapperServiceExtensionsTemplate = LoadTemplate("DataRecordMapperServiceCollectionExtensions.scriban-cs");
            InitializationContext = new InitializationContext(dataMapperTemplate, dataMapperServiceExtensionsTemplate);
        }
        
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
            if (!(context.Node is TypeDeclarationSyntax classDeclarationSyntax)) return;
            if (classDeclarationSyntax.AttributeLists.Count <= 0) return;
            if (!classDeclarationSyntax.AttributeLists.HasGenerateAttributes()) return;
                
            GenerateMapperData.AddRange(FilterGenerateAttributes(classDeclarationSyntax.AttributeLists, context.SemanticModel)
                    .Select(a => GetMapperData(classDeclarationSyntax, a, context.SemanticModel)));
        }
        
        private GenerateMapperData GetMapperData(TypeDeclarationSyntax classDeclarationSyntax, AttributeSyntax attribute, SemanticModel model) {
            var mapperData = new GenerateMapperData();
            if (attribute.ArgumentList != null && attribute.ArgumentList.Arguments.Count == 1) {
                mapperData.EntityTypes = attribute.ArgumentList.Arguments[0].GetNamedTypeSymbol(model)
                        .Select(s => GetEntityMapperData(s, mapperData.NamingPolicy)).ToArray();
            } else if (attribute.ArgumentList != null && attribute.ArgumentList.Arguments.Count == 2) {
                var policyId = (int?)attribute.ArgumentList.Arguments[1].GetConstantValue(model);
                switch (policyId) {
                    case 0:
                    case 1:
                        mapperData.NamingPolicy = PropertyNamingPolicy.SnakeCase;
                        break;
                    default:
                        throw new ArgumentException("Invalid policy id");
                }
                mapperData.EntityTypes = attribute.ArgumentList.Arguments[0].GetNamedTypeSymbol(model)
                        .Select(s => GetEntityMapperData(s, mapperData.NamingPolicy)).ToArray();
            } else {
                var definedClassSymbol = (INamedTypeSymbol)model.GetDeclaredSymbol(classDeclarationSyntax)!;
                mapperData.EntityTypes = new[] { GetEntityMapperData(definedClassSymbol, mapperData.NamingPolicy) };
            }
            return mapperData;
        }

        private GenerateMapperEntity GetEntityMapperData(INamedTypeSymbol typeSymbol, PropertyNamingPolicy namingPolicy) {
            var entity = new GenerateMapperEntity();
            var propertySymbols = typeSymbol
                    .GetBaseTypesAndThis()
                    .SelectMany(s => s.GetMembers())
                    .OfType<IPropertySymbol>()
                    .Where(p => p.GetMethod != null && p.Name != "EqualityContract").ToList();
            var tenantIdProperty = propertySymbols
                    .Where(s => s.HasAttribute(InitializationContext.TenantIdColumnAttributeName))
                    .Select(s => CreateEntityProperty(s, namingPolicy))
                    .FirstOrDefault();
            var properties = propertySymbols
                    .Select(s => CreateEntityProperty(s, namingPolicy)).ToList();
            var fullTypeName = typeSymbol.ToString();
            var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            var entityName = typeName;
            if (entityName.EndsWith(InitializationContext.EntitySuffix)) {
                entityName = entityName.Substring(0, entityName.Length - InitializationContext.EntitySuffix.Length);
            }
            var tableName = namingPolicy.ConvertName(entityName);
            entity.TenantIdProperty = tenantIdProperty;
            entity.Namespace = typeSymbol.ContainingNamespace.ToString();
            entity.TypeName = typeName;
            entity.FullTypeName = fullTypeName;
            entity.TableName = tableName;
            entity.EntityName = entityName;
            entity.Properties = properties;
            return entity;
        }

        private MapperEntityProperty CreateEntityProperty(IPropertySymbol propertySymbol, PropertyNamingPolicy namingPolicy) {
            var isJson = propertySymbol.HasAttribute(InitializationContext.JsonColumnAttributeName);
            return new MapperEntityProperty() {
                    PropertyName = propertySymbol.Name,
                    ColumnName = namingPolicy.ConvertName(propertySymbol.Name),
                    TypeName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    IsJson = isJson
            };
        }
        
        private IEnumerable<AttributeSyntax> FilterGenerateAttributes(SyntaxList<AttributeListSyntax> attributes, SemanticModel model) {
            return attributes.SelectMany(x => x.Attributes)
                    .Where(attr => {
                        // Attribute with parameters (MethodSymbol) CaaS.Mapper.GenerateMapper.GenerateMapper(System.Type, CaaS.Mapper.PropertyMappingPolicy)
                        var attrSymbol = model.GetSymbolInfo(attr).Symbol;
                        if (attrSymbol == null) return false;
                        // Attribute Type CaaS.Mapper.GenerateMapper
                        var attrTypeSymbol = attrSymbol.ContainingSymbol;
                        return attrTypeSymbol!.ToString() == InitializationContext.GenerateMapperAttributeName;
                    });
        }

        private Template LoadTemplate(string name) {
            string templateString;
            using (var templateStream = new StreamReader(typeof(CaasSourceGenerator).Assembly
                                   .GetManifestResourceStream($"CaaS.Infrastructure.Generator.{name}")
                           ?? throw new FileNotFoundException($"Can't load {name} template"))) {
                templateString = templateStream.ReadToEnd();
            }
            return Template.Parse(templateString, name);
        }
    }
}