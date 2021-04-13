namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0002Analyzer : AnalyzerBase
    {
        private const string DefaultMainMethodName = "Main";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MethodDeclarationSyntax methodDeclarationSyntax)
            {
                return;
            }

            if (context.ContainingSymbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (methodSymbol.Name.EndsWith("Async"))
            {
                return;
            }

            // Ignore Main method
            if (methodSymbol.IsStatic && string.Equals(methodSymbol.Name, DefaultMainMethodName))
            {
                return;
            }

            if (!methodDeclarationSyntax.ReturnType.IsTask(context))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var diagnosticLocation = methodSymbol.Locations.FirstOrDefault();

            if (diagnosticLocation is null)
            {
                return;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptors.CL0002_UseAsyncSuffixForAsyncMethods,
                    diagnosticLocation));
        }
    }
}
