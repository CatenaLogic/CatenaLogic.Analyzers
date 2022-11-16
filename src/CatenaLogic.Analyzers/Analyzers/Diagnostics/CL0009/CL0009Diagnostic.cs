namespace CatenaLogic.Analyzers
{
    using System.Linq;
    using System.Threading;
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

            // Check: can be not a compile-time value
            if (operation.Parent is not null)
            {
                if (operation.Parent.Kind == OperationKind.ParameterInitializer)
                {
                    return;
                }

                if (context.Operation.Parent is IFieldInitializerOperation fieldInitializer)
                {
                    var fieldSymbol = fieldInitializer.InitializedFields.FirstOrDefault();
                    if (fieldSymbol is null || fieldSymbol.IsConst)
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0009_StringEmptyIsRecommended, operation.Syntax.GetLocation()));
        }

        private bool CanHandleSyntaxNode(LiteralExpressionSyntax? syntaxNode)
        {
            return syntaxNode is not null && string.IsNullOrEmpty(syntaxNode.Token.ValueText);
        }
    }
}
