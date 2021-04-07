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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0002CodeFixProvider))]
    public sealed class CL0002CodeFixProvider : CodeFixProvider
    {
        private const string Title = "Fix method name";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0002_UseAsyncSuffixForAsyncMethods.Id);

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

            // Find the syntax identified by the diagnostic.
            var diagnosticNode = root.FindNode(diagnosticSpan);

            if (diagnosticNode is not MethodDeclarationSyntax methodDeclatarionSyntax)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
              CodeAction.Create(Title, c =>
              FixRegexAsync(context.Document, methodDeclatarionSyntax, c), equivalenceKey: Title), diagnostic);
        }

        private async Task<Document> FixRegexAsync(Document document, MethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
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

            var methodIdentifier = declarationSyntax.ChildTokens().FirstOrDefault(token => token.Kind() == SyntaxKind.IdentifierToken);

            var fixedMethodIdentifier = SyntaxFactory.Identifier(methodIdentifier.Text + "Async");

            var newRoot = root.ReplaceToken(methodIdentifier, fixedMethodIdentifier);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}
