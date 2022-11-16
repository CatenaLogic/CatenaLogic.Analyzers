namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CL0011Diagnostic : DiagnosticBase
    {
        public const string Id = "CL0011";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ThrowStatementSyntax syntax)
            {
                return;
            }

            // Followed by object creation pattern
            var objectCreationSyntax = syntax.ChildNodes().FirstOrDefault(s => s.IsKind(SyntaxKind.ObjectCreationExpression));
            if (objectCreationSyntax is null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0011_ProvideCatelLogOnThrowingException, context.Node.GetLocation()));
        }
    }
}
