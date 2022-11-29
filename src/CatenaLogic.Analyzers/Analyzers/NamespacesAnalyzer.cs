namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NamespacesAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly OperationKind[] TriggerOperations = new[]
        {
            OperationKind.DeclarationPattern,
            OperationKind.UsingDeclaration,
            OperationKind.DeclarationExpression,
            OperationKind.Block,
        };

        private static readonly SymbolKind[] TriggerSymbols = new[]
        {
            SymbolKind.Namespace
        };

        private static readonly SyntaxKind[] TriggerSyntaxNodes = new[]
        {
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.NamespaceKeyword
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Descriptors.CL0003_DontUseExtensionsNamespace,
                Descriptors.CL0004_DontUseInterfacesNamespace,
                Descriptors.CL0005_DontUseHelpersNamespace,
                Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile);

        protected override OperationKind[] GetTriggerOperations()
        {
            return TriggerOperations;
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return TriggerSymbols;
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return TriggerSyntaxNodes;
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var memberSymbol = context.ContainingSymbol;
            if (memberSymbol is null || (memberSymbol.Kind != SymbolKind.Namespace && memberSymbol.Kind != SymbolKind.NamedType))
            {
                return false;
            }

            return true;
        }
    }
}
