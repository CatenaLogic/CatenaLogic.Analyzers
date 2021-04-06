namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0003Analyzer : AnalyzerBase
    {
        private readonly static string HandledNamespace = "Extension";
        private readonly static string HandledNamespacePlural = "Extensions";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDeclarationSyntaxNode)
            {
                return;
            }

            if (context.ContainingSymbol is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var namespaceSymbol = context.ContainingSymbol.ContainingNamespace;

            if (namespaceSymbol.Name.EndsWith(HandledNamespace, System.StringComparison.OrdinalIgnoreCase) ||
                namespaceSymbol.Name.EndsWith(HandledNamespacePlural, System.StringComparison.OrdinalIgnoreCase))
            {
                if (!namespaceSymbol.OriginalDefinition.Name.EndsWith(namespaceSymbol.Name))
                {
                    return;
                }

                var reportLocation = namespaceSymbol.Locations.FirstOrDefault(l => l.SourceTree == classDeclarationSyntaxNode.GetLocation().SourceTree);

                if(reportLocation is null)
                {
                    // Cannot locate file for report, probably metadata?
                    return;
                }

                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CL0003_DontUseExtensionsNamespace, reportLocation));
            }
        }
    }
}
