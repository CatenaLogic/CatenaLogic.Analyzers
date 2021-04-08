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
    using Microsoft.CodeAnalysis.Rename;

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
              FixMethodNameAsync(context.Document, methodDeclatarionSyntax, c), equivalenceKey: Title), diagnostic);
        }

        private async Task<Solution> FixMethodNameAsync(Document document, MethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            if (semanticModel is null)
            {
                return solution;
            }

            var root = await document.GetSyntaxRootAsync();
            if (root is null)
            {
                return solution;
            }

            var methodSymbol = semanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
            if (methodSymbol is null)
            {
                return solution;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return solution;
            }

            var modifiedSolution = await Renamer.RenameSymbolAsync(solution, methodSymbol, methodSymbol.Name + "Async", solution.Workspace.Options, cancellationToken);
            return modifiedSolution;
        }
    }
}
