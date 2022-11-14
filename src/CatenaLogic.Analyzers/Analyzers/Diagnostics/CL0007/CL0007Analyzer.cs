namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

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
            var textBlock = header.Where(x => !x.IsKind(SyntaxKind.EndOfLineTrivia));

            if (textBlock.Any(x => x.IsKind(SyntaxKind.MultiLineCommentTrivia)))
            {
                var multiLineCommentTrivia = textBlock.FirstOrDefault(x => x.IsKind(SyntaxKind.MultiLineCommentTrivia));

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
                if (textBlock.Count() < 2)
                {
                    return;
                }
            }

            var headerLocation = Location.Create(syntaxNode.SyntaxTree, header.FullSpan);

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, headerLocation));
        }
    }
}
