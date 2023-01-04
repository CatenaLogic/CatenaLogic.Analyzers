namespace CatenaLogic.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class CL0009Diagnostic : DiagnosticBase
    {
        private const int MaxOperationSearchDepth = 4;

        public const string Id = "CL0009";

        public override void HandleOperation(OperationAnalysisContext context)
        {
            var operation = context.Operation;
            if (operation.Kind != OperationKind.Literal)
            {
                return;
            }

            if (operation.Type is not null && operation.Type.Name != "string" && operation.Type.ContainingNamespace.Name != "System")
            {
                return;
            }

            if (!CanHandleSyntaxNode(operation.Syntax as LiteralExpressionSyntax))
            {
                return;
            }

            // Ignore constant expression
            // In some places "" should be compile-time constant, we can't use string.Empty.
            // Çheck both parent and 1 level higher in operation tree, if value is part of conversion
            var suspectOperation = operation.Parent;
            if (!CanHandleOperation(suspectOperation))
            {
                return;
            }

            suspectOperation = suspectOperation?.Parent;
            if (!CanHandleOperation(suspectOperation))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0009_StringEmptyIsRecommended, operation.Syntax.GetLocation()));
        }

        private static bool CanHandleOperation([MaybeNullWhen(true)] IOperation? operation)
        {
            if (operation is not null)
            {
                if (operation.Kind == OperationKind.CaseClause)
                {
                    return false;
                }

                if (operation.Kind == OperationKind.ParameterInitializer)
                {
                    return false;
                }

                if (operation.Kind == OperationKind.ArrayInitializer)
                {
                    var topOperation = FindTopMostOperation(operation, MaxOperationSearchDepth);
                    if (topOperation.Syntax.IsKind(SyntaxKind.Attribute) ||
                        topOperation.Syntax.IsKind(SyntaxKind.AttributeArgument))
                    {
                        return false;
                    }
                }

                if (operation.Kind == OperationKind.FieldInitializer && operation is IFieldInitializerOperation fieldInitializer)
                {
                    var fieldSymbol = fieldInitializer.InitializedFields.FirstOrDefault();
                    if (fieldSymbol is null || fieldSymbol.IsConst)
                    {
                        return false;
                    }
                }

                if (operation.Syntax.IsKind(SyntaxKind.Attribute) ||
                    operation.Syntax.IsKind(SyntaxKind.AttributeArgument))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanHandleSyntaxNode(LiteralExpressionSyntax? syntaxNode)
        {
            return syntaxNode is not null && string.IsNullOrEmpty(syntaxNode.Token.ValueText);
        }

        private static IOperation FindTopMostOperation(IOperation operation, int maxDepth)
        {
            var i = maxDepth;
            while (i > 0 && operation.Kind != OperationKind.None)
            {
                i--;
                var parent = operation.Parent;
                if (parent is null)
                {
                    break;
                }

                operation = parent;
            }

            return operation;
        }
    }
}
