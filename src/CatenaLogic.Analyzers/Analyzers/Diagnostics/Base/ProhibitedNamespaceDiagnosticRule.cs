namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class ProhibitedNamespaceDiagnosticRule : DiagnosticRuleBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not NamespaceDeclarationSyntax namespaceDeclarationSyntax)
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

            var originalSourceTree = namespaceDeclarationSyntax.GetLocation().SourceTree;
            if (originalSourceTree is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            HandleNamespaceSymbol(namespaceSymbol, context, namespaceDeclarationSyntax, originalSourceTree);
        }

        protected abstract void HandleNamespaceSymbol(INamespaceSymbol namespaceSymbol, SyntaxNodeAnalysisContext originalContext, NamespaceDeclarationSyntax namespaceDeclarationSyntaxNode, SyntaxTree originalSyntaxTree);
    }
}
