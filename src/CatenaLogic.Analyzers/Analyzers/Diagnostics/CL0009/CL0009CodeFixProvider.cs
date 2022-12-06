namespace CatenaLogic.Analyzers
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0009CodeFixProvider))]
    internal class CL0009CodeFixProvider : CodeFixProvider
    {
        public const string Title = "Replace with string.Empty";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0009_StringEmptyIsRecommended.Id);

        public override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnosticNode = await context.FindSyntaxNodeAsync().ConfigureAwait(false);
            if (diagnosticNode is null)
            {
                return;
            }

            if (diagnosticNode is ArgumentSyntax argument)
            {
                diagnosticNode = argument.Expression;
            }

            if (diagnosticNode is not LiteralExpressionSyntax literal || !literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            context.RegisterCodeFix(
              CodeAction.Create(Title, cancellationToken =>
              FixAsync(context.Document, literal, cancellationToken), equivalenceKey: Title), context.Diagnostics);
        }

        private static async Task<Document> FixAsync(Document document, LiteralExpressionSyntax literalSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var stringEmpty = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("string"), SyntaxFactory.IdentifierName("Empty"));
            var updatedRoot = root.ReplaceNode(literalSyntax, stringEmpty);

            return document.WithSyntaxRoot(updatedRoot);
        }
    }
}
