namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0007CodeFixProvider))]
    public class CL0007CodeFixProvider : CodeFixProvider
    {
        private const string Title = @"Remove file header";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile.Id);

        public override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnosticToken = await context.FindSyntaxTokenAsync().ConfigureAwait(false);
            if (diagnosticToken == default)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Find the parent equals expression
            var equalsExpression = diagnosticToken.Parent?.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().First();
            if (equalsExpression is null)
            {
                return;
            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
              CodeAction.Create(Title, c =>
              FixAsync(context.Document, equalsExpression, diagnosticToken.Kind(), c), equivalenceKey: Title), context.Diagnostics);
        }

        private async Task<Document> FixAsync(Document document, BinaryExpressionSyntax expresssionSyntax, SyntaxKind tokenKind, CancellationToken cancellationToken)
        {
            return document;
        }
    }
}
