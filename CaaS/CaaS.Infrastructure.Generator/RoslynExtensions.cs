using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CaaS.Infrastructure.Generator {
    internal static class RoslynExtensions {
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type) {
            var current = type;
            while (current != null) {
                yield return current;
                current = current.BaseType;
            }
        }
        
        public static bool HasGenerateAttributes(this SyntaxList<AttributeListSyntax> attributes) {
            return attributes.Any(a => a.Attributes
                    .Any(x => x.Name.ToString() == InitializationContext.GenerateMapperSimpleAttributeName));
        }

        public static INamedTypeSymbol[] GetAttributeTypes(this AttributeArgumentListSyntax attributes, SemanticModel model) {
            return attributes.Arguments.Select(a => a.Expression)
                    .SelectMany(e => e.GetNamedTypeSymbol(model)).ToArray();
        }

        public static object? GetConstantValue(this AttributeArgumentSyntax argument, SemanticModel model) {
            return argument.Expression.GetConstantValue(model);
        }
        
        public static IEnumerable<INamedTypeSymbol> GetNamedTypeSymbol(this AttributeArgumentSyntax argument, SemanticModel model) {
            return argument.Expression.GetNamedTypeSymbol(model);
        }
        
        public static object? GetConstantValue(this ExpressionSyntax expression, SemanticModel model) {
            if (expression is LiteralExpressionSyntax literalExpressionSyntax) {
                return literalExpressionSyntax.Token.Value;
            }
            return null;
        }

        public static IEnumerable<INamedTypeSymbol> GetNamedTypeSymbol(this ExpressionSyntax expression, SemanticModel model) {
            if (expression is TypeOfExpressionSyntax typeOfExpressionSyntax) {
                var symbolInfo = model.GetSymbolInfo(typeOfExpressionSyntax.Type);
                var symbolInfoSymbol = (INamedTypeSymbol)symbolInfo.Symbol!;
                return new[] { symbolInfoSymbol };
            }
            if (expression is ArrayCreationExpressionSyntax arrayCreationSyntax) {
                return arrayCreationSyntax.Initializer!.Expressions
                        .SelectMany(e => e.GetNamedTypeSymbol(model));
            }
            return Enumerable.Empty<INamedTypeSymbol>();
        }
    }
}