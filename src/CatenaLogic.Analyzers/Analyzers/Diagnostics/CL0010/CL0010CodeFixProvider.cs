namespace CatenaLogic.Analyzers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0010CodeFixProvider))]
    internal class CL0010CodeFixProvider : CodeFixProvider
    {
        public const string Title = "Remove region directive";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0010_DontKeepClassMemberRegions.Id);

        public override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnosticNode = await context.FindSyntaxNodeAsync().ConfigureAwait(false);
            if (diagnosticNode == default)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic is null)
            {
                return;
            }

            context.RegisterCodeFix(
              CodeAction.Create(Title, cancellationToken =>
              FixAsync(context.Document, diagnosticNode, diagnostic.AdditionalLocations, cancellationToken), equivalenceKey: Title), context.Diagnostics);
        }

        private static async Task<Document> FixAsync(Document document, SyntaxNode node, IReadOnlyList<Location> locations, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var startText = await document.GetTextAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return document;
            }

            var listOfChanges = new List<TextChange>();
            foreach (var location in locations)
            {
                var lineSpan = location.GetLineSpan();
                var line = startText.Lines[lineSpan.StartLinePosition.Line];

                // Replace whole line with line breaks with empty
                listOfChanges.Add(new TextChange(line.SpanIncludingLineBreak, string.Empty));
            }

            startText = startText.WithChanges(listOfChanges);

            return document.WithText(startText);
        }
    }
}
