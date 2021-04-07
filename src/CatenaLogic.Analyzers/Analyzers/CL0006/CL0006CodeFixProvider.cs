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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0006CodeFixProvider))]
    public class CL0006CodeFixProvider : CodeFixProvider
    {
        private const string Title = @"Change equality check to ""is"" statement";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck.Id);

        public override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the expression token identified by the diagnostic.
            var diagnsoticToken = root.FindToken(diagnosticSpan.Start);

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Find the parent equals expression
            var equalsExpression = diagnsoticToken.Parent?.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().First();

            if (equalsExpression is null)
            {
                return;
            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
              CodeAction.Create(Title, c =>
              FixRegexAsync(context.Document, equalsExpression, diagnsoticToken.Kind(), c), equivalenceKey: Title), diagnostic);
        }

        private async Task<Document> FixRegexAsync(Document document, BinaryExpressionSyntax expresssionSyntax, SyntaxKind tokenKind, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            if (semanticModel is null)
            {
                return document;
            }

            var root = await document.GetSyntaxRootAsync();
            if (root is null)
            {
                return document;
            }

            var leftPart = expresssionSyntax.Left;

            // Replacing right part
            var replacementPattern = expresssionSyntax.Right;

            var nullExpression = SyntaxFactory.ParseExpression("null");

            if (tokenKind == SyntaxKind.EqualsEqualsToken)
            {
                replacementPattern = SyntaxFactory.IsPatternExpression(leftPart, SyntaxFactory.ConstantPattern(nullExpression));
            }
            else
            {
                replacementPattern = SyntaxFactory.IsPatternExpression(leftPart, SyntaxFactory.UnaryPattern(SyntaxFactory.Token(SyntaxKind.NotKeyword), SyntaxFactory.ConstantPattern(nullExpression)));
            }

            var newRoot = root.ReplaceNode(expresssionSyntax, replacementPattern);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}
