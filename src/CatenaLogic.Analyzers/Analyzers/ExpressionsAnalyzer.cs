namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning restore RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    internal class ExpressionsAnalyzer : DiagnosticAnalyzerBase
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
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
