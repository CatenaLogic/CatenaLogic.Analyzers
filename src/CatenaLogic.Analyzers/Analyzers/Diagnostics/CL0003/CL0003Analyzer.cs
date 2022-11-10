namespace CatenaLogic.Analyzers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0003Analyzer : ProhibitedNamespaceDiagnosticRule
    {
        private readonly static string HandledNamespace = "Extension";
        private readonly static string HandledNamespacePlural = "Extensions";

        protected override void HandleNamespaceSymbol(INamespaceSymbol namespaceSymbol, SyntaxNodeAnalysisContext originalContext, NamespaceDeclarationSyntax namespaceDeclarationSyntaxNode, SyntaxTree originalSyntaxTree)
        {
            if (namespaceSymbol.Name.EndsWith(HandledNamespace, StringComparison.OrdinalIgnoreCase) ||
                namespaceSymbol.Name.EndsWith(HandledNamespacePlural, StringComparison.OrdinalIgnoreCase))
            {
                if (!namespaceSymbol.OriginalDefinition.Name.EndsWith(namespaceSymbol.Name))
                {
                    return;
                }

                var reportLocation = namespaceSymbol.Locations.FirstOrDefault(l => l.SourceTree == namespaceDeclarationSyntaxNode.GetLocation().SourceTree);

                if (reportLocation is null)
                {
                    // Cannot locate file for report, probably metadata?
                    return;
                }

                if (originalContext.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                originalContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CL0003_DontUseExtensionsNamespace, reportLocation));
            }
        }
    }
}
