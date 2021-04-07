namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ExpressionsAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck);

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax)
            {
                return false;
            }

            if (context.Node is not MemberDeclarationSyntax)
            {
                return false;
            }

            return true;
        }
    }
}
