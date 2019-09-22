﻿namespace CatenaLogic.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class CL0001Analyzer : AnalyzerBase
    {
        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = context.ContainingSymbol as IMethodSymbol;
            if (methodSymbol is null)
            {
                return;
            }

            var expression = context.Node as InvocationExpressionSyntax;
            if (expression is null)
            {
                return;
            }

            var memberAccessExpression = expression.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression is null)
            {
                return;
            }

            // Check if the parent method returns a Task
            var parentMethod = expression.FirstAncestor<MethodDeclarationSyntax>();
            if (parentMethod is null)
            {
                return;
            }

            if (parentMethod.ReturnType != KnownSymbols.Task &&
                !parentMethod.ReturnType.IsAssignableTo(KnownSymbols.Task, context.SemanticModel))
            {
                return;
            }

            // Check if this is actually an async method (not a sync, task returning call)
            if (!parentMethod.Modifiers.Any(x => x.Value.Equals("async")))
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

            if (context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is IMethodSymbol memberAccessSymbolInfo)
            {
                var expectedAsyncOverloadName = $"{memberAccessExpression.Name.Identifier.Value}Async";

                bool overloadFound = false;
                foreach (var methodSymbolInfo in typeSymbol.GetMembers(expectedAsyncOverloadName).OfType<IMethodSymbol>())
                {
                    if (memberAccessSymbolInfo.Parameters.Length != methodSymbolInfo.Parameters.Length)
                    {
                        continue;
                    }

                    bool parametersMatch = true;
                    for (int i = 0; i < memberAccessSymbolInfo.Parameters.Length; i++)
                    {
                        if (memberAccessSymbolInfo.Parameters[i].Type.Name != methodSymbolInfo.Parameters[i].Type.Name)
                        {
                            parametersMatch = false;
                            break;
                        }
                    }

                    if (parametersMatch)
                    {
                        overloadFound = true;
                        break;
                    }
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
