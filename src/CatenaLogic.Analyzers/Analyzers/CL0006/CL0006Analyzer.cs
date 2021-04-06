namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class CL0006Analyzer : AnalyzerBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not BinaryExpressionSyntax binaryExpressionSyntax)
            {
                return;
            }

            if (binaryExpressionSyntax.Kind() != SyntaxKind.EqualsExpression && binaryExpressionSyntax.Kind() != SyntaxKind.NotEqualsExpression)
            {
                return;
            }

            var rightOperand = binaryExpressionSyntax.Right;

            if (rightOperand is not LiteralExpressionSyntax rightOperandLiteral)
            {
                return;
            }

            if (rightOperandLiteral.Kind() != SyntaxKind.NullLiteralExpression)
            {
                return;
            }

            context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck, binaryExpressionSyntax.GetLocation()));
        }
    }
}
