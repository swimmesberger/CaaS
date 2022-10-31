using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CaaS.Generator.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace CaaS.Generator {
    [Generator]
    public class EntityRowMapperSourceGenerator : ISourceGenerator {
        internal const string RowMapperAttributeName = nameof(GenerateMapper);
        internal const string GenerateNamespace = "CaaS.Infrastructure.Gen";

        private Template _rowMapperTemplate;
        private Template _serviceExtensionsTemplate;
        
        public void Execute(GeneratorExecutionContext context) {
            // retrieve the populated receiver 
            if (context.SyntaxReceiver is SyntaxReceiver receiver == false) {
                return;
            }
            
            var compilation = context.Compilation;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (compilation == null) {
                return;
            }
            try {
                var entitySymbols = GetEntityClasses(receiver, compilation);
                GenerateForClasses(context, entitySymbols);
            } catch (Exception e) {
                ReportError(context, e);
                throw;
            }
        }

        public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
            if (!Debugger.IsAttached) {
                // Debugger.Launch();
            }
#endif
            _rowMapperTemplate = LoadTemplate("DataRecordMapper.scriban-cs");
            _serviceExtensionsTemplate = LoadTemplate("DataRecordMapperServiceCollectionExtensions.scriban-cs");
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private Template LoadTemplate(string name) {
            string templateString;
            using (var templateStream = new StreamReader(typeof(EntityRowMapperSourceGenerator).Assembly
                                   .GetManifestResourceStream($"CaaS.Generator.{name}") 
                           ?? throw new FileNotFoundException($"Can't load {name} template"))) {
                templateString = templateStream.ReadToEnd();
            }
            return Template.Parse(templateString, name);
        }

        private void ReportError(GeneratorExecutionContext context, Exception e) {
            var descriptor = new DiagnosticDescriptor(nameof(EntityRowMapperSourceGenerator), "Error",
                    $"{nameof(EntityRowMapperSourceGenerator)} failed to generate Interface due to an error. Please inform the author. Error: {e}",
                    "Compilation", DiagnosticSeverity.Error, isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, null));
        }
        
        private void GenerateForClasses(GeneratorExecutionContext context, List<GenerateMapperData> mapperData) {
            var serviceExtensionsTemplateModel = new ServiceExtensionsTemplateModel();
            foreach (var data in mapperData) {
                foreach (var namedTypeSymbol in data.EntityTypes) {
                    var rowMapper = GenerateRowMapper(context, namedTypeSymbol, data.NamingPolicy);
                    serviceExtensionsTemplateModel.Mappers.Add(rowMapper);
                }
            }
            var sourceCode = _serviceExtensionsTemplate.Render(serviceExtensionsTemplateModel, member => member.Name);
            context.AddSource("DataRecordMapperServiceCollectionExtensions", SourceText.From(sourceCode, Encoding.UTF8));
        }
        
        private RowMapper GenerateRowMapper(GeneratorExecutionContext context, INamedTypeSymbol entityType, PropertyNamingPolicy namingPolicy) {
            try {
                var templateModel = CreateModel(entityType, namingPolicy);
                var sourceCode = _rowMapperTemplate.Render(templateModel, member => member.Name);
                var typeName = $"{templateModel.Entity.TypeName}DataRecordMapper";
                // File.WriteAllText(@"C:\studium\Material\5. Semester\Studienprojekt\caas-bb-g2-wimmesberger-g2-kofler-hofer\CaaS\CaaS.Infrastructure\" + typeName + ".cs", sourceCode);
                context.AddSource(typeName, SourceText.From(sourceCode, Encoding.UTF8));
                return new RowMapper() {
                    Name = typeName,
                    EntityType = templateModel.Entity.TypeName,
                    EntityNamespace = templateModel.Entity.Namespace
                };
            } catch (Exception e) {
                ReportError(context, e);
                throw;
            }
        }
        
        private static RowMapperTemplateModel CreateModel(INamedTypeSymbol typeSymbol, PropertyNamingPolicy namingPolicy) {
            var properties = typeSymbol.GetBaseTypesAndThis().SelectMany(s => s.GetMembers())
                    .OfType<IPropertySymbol>()
                    .Where(p => p.GetMethod != null && p.Name != "EqualityContract")
                    .Select(s => new EntityModelProperty() {
                            PropertyName = s.Name,
                            ColumnName = namingPolicy.ConvertName(s.Name),
                            TypeName = s.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                    });
            var fullTypeName = typeSymbol.ToString();
            var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            return new RowMapperTemplateModel() {
                Entity = new EntityModel() {
                    Namespace = typeSymbol.ContainingNamespace.ToString(),
                    TypeName = typeName,
                    FullTypeName = fullTypeName,
                    TableName = namingPolicy.ConvertName(typeName),
                    Properties = properties.ToList()       
                } 
            };
        }

        private static List<GenerateMapperData> GetEntityClasses(SyntaxReceiver receiver, Compilation compilation) {
            return receiver.CandidateClasses.SelectMany(cls => {
                var model = compilation.GetSemanticModel(cls.SyntaxTree);
                return cls.AttributeLists
                        .FilterGenerateAttributes()
                        .Select(a => GetMapperData(a, model));
            }).ToList();
        }

        private static GenerateMapperData GetMapperData(AttributeSyntax attribute, SemanticModel model) {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count <= 0) {
                throw new Exception("No entity type specified");
            }
            PropertyNamingPolicy namingPolicy;
            if (attribute.ArgumentList.Arguments.Count == 1) {
                namingPolicy = PropertyNamingPolicy.SnakeCase;
            } else {
                var namedType = attribute.ArgumentList.Arguments[1].GetNamedTypeSymbol(model);
                var namingPolicyType = Type.GetType(namedType.ToString());
                namingPolicy = (PropertyNamingPolicy)Activator.CreateInstance(namingPolicyType!);
            }
            var attributeTypeValues = attribute.ArgumentList.GetAttributeTypes(model);
            if (attributeTypeValues.Length <= 0) {
                throw new Exception("No entity type specified");
            }
            INamedTypeSymbol[] entityTypes = attribute.ArgumentList.Arguments[0].GetNamedTypeSymbol(model);
            return new GenerateMapperData() {
                    EntityTypes = entityTypes,
                    NamingPolicy = namingPolicy
            };
        }
    }

    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    internal class SyntaxReceiver : ISyntaxReceiver {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && 
                classDeclarationSyntax.AttributeLists.HasGenerateAttributes()) {
                CandidateClasses.Add(classDeclarationSyntax);
            }
        }
    }

    internal class GenerateMapperData {
        public INamedTypeSymbol[] EntityTypes { get; set; }
        public PropertyNamingPolicy NamingPolicy { get; set; }
    }

    internal class ServiceExtensionsTemplateModel {
        public string Namespace { get; set; } = EntityRowMapperSourceGenerator.GenerateNamespace;
        public List<RowMapper> Mappers { get; } = new List<RowMapper>();
        public IEnumerable<string> UsingNamespaces => Mappers.Select(m => m.EntityNamespace).Distinct();
    }

    internal class RowMapperTemplateModel {
        public string Namespace { get; set; } = EntityRowMapperSourceGenerator.GenerateNamespace;
        public EntityModel Entity { get; set; }
    }

    internal class EntityModel {
        public string Namespace { get; set; }
        public string TypeName { get; set; }
        public string FullTypeName { get; set; }
        public string TableName { get; set; }
        public IEnumerable<EntityModelProperty> Properties { get; set; }
    }

    internal class EntityModelProperty {
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public string TypeName { get; set; }
    }
    
    internal class RowMapper {
        public string Name { get; set; }
        public string EntityType { get; set; }
        public string EntityNamespace { get; set; }
    }
    
    internal static class RoslynExtensions {
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type) {
            var current = type;
            while (current != null) {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<AttributeSyntax> FilterGenerateAttributes(this SyntaxList<AttributeListSyntax> attributes) {
            return attributes.SelectMany(x => x.Attributes)
                    .Where(attr => 
                            attr.Name.ToString() == EntityRowMapperSourceGenerator.RowMapperAttributeName);
        }
        
        public static bool HasGenerateAttributes(this SyntaxList<AttributeListSyntax> attributes) {
            return attributes.Any(a => a.Attributes
                    .Any(x => x.Name.ToString() == EntityRowMapperSourceGenerator.RowMapperAttributeName));
        }

        public static INamedTypeSymbol[] GetAttributeTypes(this AttributeArgumentListSyntax attributes, SemanticModel model) {
            return attributes.Arguments.Select(a => a.Expression)
                    .SelectMany(e => e.GetNamedTypeSymbol(model)).ToArray();
        }

        public static INamedTypeSymbol[] GetNamedTypeSymbol(this AttributeArgumentSyntax argument, SemanticModel model) {
            return argument.Expression.GetNamedTypeSymbol(model);
        }

        public static INamedTypeSymbol[] GetNamedTypeSymbol(this ExpressionSyntax expression, SemanticModel model) {
            if (expression is TypeOfExpressionSyntax typeOfExpressionSyntax) {
                var symbolInfo = model.GetSymbolInfo(typeOfExpressionSyntax.Type);
                var symbolInfoSymbol = (INamedTypeSymbol)symbolInfo.Symbol;
                return new INamedTypeSymbol[]{symbolInfoSymbol};
            }
            if (expression is ArrayCreationExpressionSyntax arrayCreationSyntax) {
                return arrayCreationSyntax.Initializer!.Expressions
                        .SelectMany(e => e.GetNamedTypeSymbol(model))
                        .ToArray();
            }
            return Array.Empty<INamedTypeSymbol>();
        }
    }
}