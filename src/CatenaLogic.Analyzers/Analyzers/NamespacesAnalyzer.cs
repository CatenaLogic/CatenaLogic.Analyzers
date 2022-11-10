namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NamespacesAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Descriptors.CL0003_DontUseExtensionsNamespace, 
                Descriptors.CL0004_DontUseInterfacesNamespace, 
                Descriptors.CL0005_DontUseHelpersNamespace,
                Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile);

        protected override OperationKind[] GetTriggerOperations()
        {
            return new[] {
                OperationKind.DeclarationPattern,
                OperationKind.UsingDeclaration,
                OperationKind.DeclarationExpression,
                OperationKind.Block,
            };
        }

        protected override SymbolKind[] GetTriggerSymbols()
        {
            return new[] { SymbolKind.Namespace };
        }

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return new[] { SyntaxKind.NamespaceDeclaration, SyntaxKind.NamespaceKeyword };
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
