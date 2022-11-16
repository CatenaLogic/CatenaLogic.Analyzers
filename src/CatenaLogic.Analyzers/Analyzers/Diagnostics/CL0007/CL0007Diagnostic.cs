namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CL0007Diagnostic : DiagnosticBase
    {
        public const string Id = "CL0007";

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.IsKind(SyntaxKind.NamespaceDeclaration) && context.Node.HasLeadingTrivia)
            {
                HandleSyntaxNode(context.Node, context, context.CancellationToken);
            }
        }

        private void HandleSyntaxNode(SyntaxNode syntaxNode, SyntaxNodeAnalysisContext context, CancellationToken cancellationToken)
        {
            var header = syntaxNode.GetLeadingTrivia();
            var textBlocks = header.Where(x => !x.IsKind(SyntaxKind.EndOfLineTrivia));

            if (textBlocks.Any(x => x.IsKind(SyntaxKind.MultiLineCommentTrivia)))
            {
                var multiLineCommentTrivia = textBlocks.FirstOrDefault(x => x.IsKind(SyntaxKind.MultiLineCommentTrivia));

                var fileSpan = multiLineCommentTrivia.SyntaxTree?.GetLineSpan(multiLineCommentTrivia.Span);
                if (fileSpan.HasValue)
                {
                    if (fileSpan.Value.EndLinePosition.Line - fileSpan.Value.StartLinePosition.Line == 0)
                    {
                        // Don't match single-line comments
                        return;
                    }
                }
            }
            else
            {
                if (textBlocks.Count() < 2)
                {
                    return;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var headerText = header.ToFullString();
            if (!headerText.Contains("</copyright>") && !headerText.Contains("<copyright"))
            {
                return;
            }

            var headerLocation = Location.Create(syntaxNode.SyntaxTree, header.FullSpan);
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, headerLocation));
        }
    }
}
