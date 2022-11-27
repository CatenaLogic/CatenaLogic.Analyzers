namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ClassesAnalyzer : DiagnosticAnalyzerBase
    {
        private static readonly SyntaxKind[] TriggerSyntaxNodes = new[]
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.RegionDirectiveTrivia,
            SyntaxKind.EndRegionDirectiveTrivia,
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CL0010_DontKeepClassMemberRegions);

        protected override SyntaxKind[] GetTriggerSyntaxNodes()
        {
            return TriggerSyntaxNodes;
        }

        protected override bool ShouldHandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            return true;
        }
    }
}
