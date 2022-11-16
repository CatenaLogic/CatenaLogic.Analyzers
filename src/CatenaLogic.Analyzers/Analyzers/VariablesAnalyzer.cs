namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class VariablesAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0009_StringEmptyIsRecommended);

        protected override OperationKind[] GetTriggerOperations()
        {
            return new[]
             {
                OperationKind.Invocation,
                OperationKind.VariableDeclaration,
                OperationKind.VariableDeclarationGroup,
                OperationKind.VariableDeclarator,
                OperationKind.DefaultValue,
                OperationKind.Literal,
            };
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return new[]
            {
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.VariableDeclaration,
                SyntaxKind.VariableDeclarator,
                SyntaxKind.StringLiteralExpression,
            };
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return new[]
            {
                SymbolKind.Local,
                SymbolKind.Field,
                SymbolKind.Parameter,
            };
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return false;
        }

        protected override bool ShouldHandleOperation(OperationAnalysisContext context)
        {
            return true;
        }
    }
}
