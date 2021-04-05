namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0003Analyzer : AnalyzerBase
    {
        private readonly static string HandledNamespace = "Extension";
        private readonly static string HandledNamespacePlural = "Extensions";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not INamespaceSymbol namespaceSymbol)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (string.Equals(namespaceSymbol.Name, HandledNamespace, System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(namespaceSymbol.Name, HandledNamespacePlural, System.StringComparison.OrdinalIgnoreCase))
            {
                if (!namespaceSymbol.OriginalDefinition.Name.EndsWith(namespaceSymbol.Name))
                {
                    return;
                }

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CL0003_DontUseExtensionsNamespace, context.Node.GetLocation()));
            }
        }
    }
}
