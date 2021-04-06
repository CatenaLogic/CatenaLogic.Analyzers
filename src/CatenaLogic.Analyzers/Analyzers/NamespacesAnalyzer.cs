namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NamespacesAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.CL0003_DontUseExtensionsNamespace);

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
