namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CL0007Analyzer : DiagnosticRuleBase
    {
        public const string Id = "CL0007";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.IsKind(SyntaxKind.NamespaceDeclaration) && context.Node.HasLeadingTrivia)
            {
                HandleSyntaxNode(context.Node, context);
            }
        }

        private void HandleSyntaxNode(SyntaxNode syntaxNode, SyntaxNodeAnalysisContext context)
        {
            var header = syntaxNode.GetLeadingTrivia();
            if (header.Count > 2)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, context.Node.GetLocation()));
            }
        }
    }
}
