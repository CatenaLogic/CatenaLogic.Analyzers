namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
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
            var diagnosticToken = await context.FindSyntaxNodeAsync().ConfigureAwait(false);
            if (diagnosticToken == default)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (diagnosticToken is not NamespaceDeclarationSyntax namespaceSyntax)
            {
                return;
            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
              CodeAction.Create(Title, c =>
              FixAsync(context.Document, namespaceSyntax, c), equivalenceKey: Title), context.Diagnostics);
        }

        private async Task<Document> FixAsync(Document document, NamespaceDeclarationSyntax namespaceSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var fixedNamespaceSyntax = namespaceSyntax.WithoutLeadingTrivia();

            if (cancellationToken.IsCancellationRequested)
            {
                return document;
            }

            var pendingRoot = root.ReplaceNode(namespaceSyntax, fixedNamespaceSyntax);

            return document.WithSyntaxRoot(pendingRoot);
        }
    }
}
