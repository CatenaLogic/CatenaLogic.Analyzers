namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class ProhibitedNamespaceDiagnosticAnalyzerBase : AnalyzerBase
    {
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

            var originalSourceTree = classDeclarationSyntaxNode.GetLocation().SourceTree;

            if (originalSourceTree is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            HandleNamespaceSymbol(namespaceSymbol, context, classDeclarationSyntaxNode, originalSourceTree);
        }

        protected abstract void HandleNamespaceSymbol(INamespaceSymbol namespaceSymbol, SyntaxNodeAnalysisContext originalContext, ClassDeclarationSyntax classDeclarationSyntax, SyntaxTree originalSyntaxTree);
    }
}
