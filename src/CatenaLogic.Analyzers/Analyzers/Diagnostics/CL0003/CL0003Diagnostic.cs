namespace CatenaLogic.Analyzers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0003Diagnostic : ProhibitedNamespaceDiagnostic
    {
        private readonly static string HandledNamespace = "Extension";
        private readonly static string HandledNamespacePlural = "Extensions";

        private bool CanHandleIdentifier(INamespaceSymbol namespaceSymbol)
        {
            return namespaceSymbol.Name.EndsWith(HandledNamespace, StringComparison.OrdinalIgnoreCase) ||
                namespaceSymbol.Name.EndsWith(HandledNamespacePlural, StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleNamespaceSymbol(INamespaceSymbol namespaceSymbol, SyntaxNodeAnalysisContext originalContext, NamespaceDeclarationSyntax namespaceDeclarationSyntaxNode, SyntaxTree originalSyntaxTree)
        {
            var matchProhibited = false;
            if (!CanHandleIdentifier(namespaceSymbol))
            {
                var underlyingNamespaceGroup = namespaceSymbol.GetNamespaceMembers();
                foreach (var symbol in underlyingNamespaceGroup)
                {
                    if (CanHandleIdentifier(symbol))
                    {
                        matchProhibited = true;
                        namespaceSymbol = symbol;
                    }
                }
            }

            if (!matchProhibited)
            {
                return;
            }

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
