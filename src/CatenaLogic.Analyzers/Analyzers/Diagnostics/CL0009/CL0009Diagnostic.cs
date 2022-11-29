namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;

    internal class CL0009Diagnostic : DiagnosticBase
    {
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

        private bool CanHandleOperation(IOperation? operation)
        {
            if (operation is not null)
            {
                if (operation.Kind == OperationKind.ParameterInitializer)
                {
                    return false;
                }

                if (operation is IFieldInitializerOperation fieldInitializer)
                {
                    var fieldSymbol = fieldInitializer.InitializedFields.FirstOrDefault();
                    if (fieldSymbol is null || fieldSymbol.IsConst)
                    {
                        return false;
                    }
                }

                if (operation.Syntax.IsKind(SyntaxKind.Attribute))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanHandleSyntaxNode(LiteralExpressionSyntax? syntaxNode)
        {
            return syntaxNode is not null && string.IsNullOrEmpty(syntaxNode.Token.ValueText);
        }
    }
}
