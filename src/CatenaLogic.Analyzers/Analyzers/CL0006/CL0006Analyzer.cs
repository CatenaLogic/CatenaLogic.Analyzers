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

            foreach (var binaryExpressionSyntax in descendants)
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

                //note: Check is it part of simple lambda for an Expression parameter
                var lambdaNode = FindParentExpresson<SimpleLambdaExpressionSyntax>(binaryExpressionSyntax, 7);
                if (lambdaNode is not null)
                {
                    var argumentTypeInfo = context.SemanticModel.GetTypeInfo(lambdaNode);
                    if (argumentTypeInfo.ConvertedType is null)
                    {
                        continue;
                    }

                    if (argumentTypeInfo.ConvertedType.Name == "Expression" &&
                        string.Equals(argumentTypeInfo.ConvertedType.ContainingNamespace.ToString(), "System.Linq.Expressions"))
                    {
                        return;
                    }
                }
                else
                {
                    // Ignore inside query 
                    var queryBody = binaryExpressionSyntax.Ancestors(false).FirstOrDefault(q => q is QueryBodySyntax) as QueryBodySyntax;
                    if (queryBody is not null)
                    {
                        return;
                    }
                }


                var reportedToken = binaryExpressionSyntax.ChildTokens().FirstOrDefault(token => token.Kind() == SyntaxKind.EqualsEqualsToken || token.Kind() == SyntaxKind.ExclamationEqualsToken);

                context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck, reportedToken.GetLocation()));
            }
        }

        private T? FindParentExpresson<T>(SyntaxNode node, int depth) where T : ExpressionSyntax
        {
            if (depth == 0)
            {
                return null;
            }

            var parent = node.Parent;
            if (parent is null)
            {
                return null;
            }

            if (parent is T typedParentExpresssion)
            {
                return typedParentExpresssion;
            }

            return FindParentExpresson<T>(parent, depth - 1);
        }
    }
}
