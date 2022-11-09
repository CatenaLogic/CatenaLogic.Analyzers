namespace CatenaLogic.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    public class CL0001Analyzer : DiagnosticRuleBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var expression = context.Node as InvocationExpressionSyntax;
            if (expression is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var memberAccessExpression = expression.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Check if the parent method returns a Task
            var parentMethod = expression.FirstAncestor<MethodDeclarationSyntax>();
            if (parentMethod is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!parentMethod.ReturnType.IsTask(context))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Check if this is actually an async method (not a sync, task returning call)
            if (!parentMethod.Modifiers.Any(x => x.Value?.Equals("async") ?? false))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Check if there is an async overload for this method
            // In case of File.ReadAllText, this is the data:
            //   memberAccessExpression.Expression => File
            //   memberAccessExpression.Name => ReadAllText
            var identifierNameSyntax = memberAccessExpression.Expression as IdentifierNameSyntax;
            if (identifierNameSyntax is null)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierNameSyntax);
            var typeSymbol = symbolInfo.Symbol as ITypeSymbol;
            if (typeSymbol is null)
            {
                // Try and see if this is a local variable that we can resolve
                if (symbolInfo.Symbol is ILocalSymbol localSymbol)
                {
                    typeSymbol = localSymbol.Type;
                }

                if (symbolInfo.Symbol is IFieldSymbol fieldSymbol)
                {
                    typeSymbol = fieldSymbol.Type;
                }

                if (typeSymbol is null)
                {
                    return;
                }
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is IMethodSymbol memberAccessSymbolInfo)
            {
                var expectedAsyncOverloadName = $"{memberAccessExpression.Name.Identifier.Value}Async";

                var overloadFound = false;
                foreach (var overloadMethodSymbolInfo in typeSymbol.GetMembers(expectedAsyncOverloadName).OfType<IMethodSymbol>())
                {
                    var expectedParametersCount = memberAccessSymbolInfo.Parameters.Length;
                    if (overloadMethodSymbolInfo.Parameters.Length > 0)
                    {
                        var lastParameterType = overloadMethodSymbolInfo.Parameters[overloadMethodSymbolInfo.Parameters.Length - 1].Type;
                        if (lastParameterType.ToString() == "System.Threading.CancellationToken")
                        {
                            expectedParametersCount++;
                        }
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (expectedParametersCount != overloadMethodSymbolInfo.Parameters.Length)
                    {
                        continue;
                    }

                    var parametersMatch = true;
                    for (var i = 0; i < memberAccessSymbolInfo.Parameters.Length; i++)
                    {
                        if (!SymbolEqualityComparer.Default.Equals(memberAccessSymbolInfo.Parameters[i].Type, overloadMethodSymbolInfo.Parameters[i].Type))
                        {
                            parametersMatch = false;
                            break;
                        }
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (parametersMatch)
                    {
                        overloadFound = true;
                        break;
                    }
                }

                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (overloadFound)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CL0001_UseAsyncOverloadInsideAsyncMethods,
                            expression.GetLocation()));
                }
            }
        }
    }
}
