namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0006Analyzer : AnalyzerBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MemberDeclarationSyntax memberDeclarationSyntax)
            {
                return;
            }

            // Optimize this query
            var descendants = memberDeclarationSyntax.DescendantNodes(d => true).Where(node => node is BinaryExpressionSyntax).Cast<BinaryExpressionSyntax>().ToList();

            foreach(var binaryExpressionSyntax in descendants)
            {
                if (binaryExpressionSyntax is null)
                {
                    continue;
                }

                if (binaryExpressionSyntax.Kind() != SyntaxKind.EqualsExpression && binaryExpressionSyntax.Kind() != SyntaxKind.NotEqualsExpression)
                {
                    continue;
                }

                var rightOperand = binaryExpressionSyntax.Right;
                if (rightOperand is not LiteralExpressionSyntax rightOperandLiteral)
                {
                    continue;
                }

                if (rightOperandLiteral.Kind() != SyntaxKind.NullLiteralExpression)
                {
                    continue;
                }

                var reportedToken = binaryExpressionSyntax.ChildTokens().FirstOrDefault(token => token.Kind() == SyntaxKind.EqualsEqualsToken || token.Kind() == SyntaxKind.ExclamationEqualsToken);

                context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck, reportedToken.GetLocation()));
            }
        }
    }
}
