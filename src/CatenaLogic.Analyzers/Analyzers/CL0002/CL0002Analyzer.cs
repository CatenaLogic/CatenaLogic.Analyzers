namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0002Analyzer : AnalyzerBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!methodSymbol.IsAsync)
            {
                return;
            }

            if (methodSymbol.Name.EndsWith("Async"))
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
