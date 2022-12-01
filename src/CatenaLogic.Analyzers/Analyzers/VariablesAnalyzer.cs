namespace CatenaLogic.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class VariablesAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly OperationKind[] TriggerOperations = new[]
        {
            OperationKind.Invocation,
            OperationKind.VariableDeclaration,
            OperationKind.VariableDeclarationGroup,
            OperationKind.VariableDeclarator,
            OperationKind.DefaultValue,
            OperationKind.Literal,
        };

        private static readonly SymbolKind[] TriggerSymbols = new[]
        {
            SymbolKind.Local,
            SymbolKind.Field,
            SymbolKind.Parameter,
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0009_StringEmptyIsRecommended);

        protected override OperationKind[] GetTriggerOperations()
        {
            return TriggerOperations;
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return Array.Empty<SyntaxKind>();
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return TriggerSymbols;
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
