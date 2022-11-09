using System.Diagnostics;
using Microsoft.CodeAnalysis;

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace CaaS.Infrastructure.Generator {
    [Generator]
    public class CaasSourceGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
            if (!Debugger.IsAttached) {
                // Debugger.Launch();
            }
#endif

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            // retrieve the populated receiver 
            if (context.SyntaxContextReceiver is SyntaxReceiver receiver == false) {
                return;
            }

            new SourceGenerator(context, receiver).Generate();
        }
    }
}