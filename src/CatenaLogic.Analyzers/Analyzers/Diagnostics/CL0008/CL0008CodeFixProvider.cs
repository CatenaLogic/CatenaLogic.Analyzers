namespace CatenaLogic.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CL0008CodeFixProvider))]
    internal class CL0008CodeFixProvider : CodeFixProvider
    {
        public const string Title = "Replace with ArgumentNullException.ThrowIfNull";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Descriptors.CL0008_DoUseThrowIfNullForArgumentCheck.Id);
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

            if (diagnosticNode is not InvocationExpressionSyntax invocationSyntax)
            {
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            context.RegisterCodeFix(
              CodeAction.Create(Title, cancellationToken =>
              FixAsync(context.Document, invocationSyntax, cancellationToken), equivalenceKey: Title), context.Diagnostics);
        }

        /// <summary>
        /// Code action changes Argument.IsNotNull expression to ArgumentNullException.ThrowIfNull invocation
        /// </summary>
        /// <param name="document"></param>
        /// <param name="invocationExpressionSyntax"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<Document> FixAsync(Document document, InvocationExpressionSyntax invocationExpressionSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var argumentNames = GetArgumentNames(invocationExpressionSyntax);
            if (!argumentNames.Any())
            {
                return document;
            }

            var isNotNullParameter = argumentNames.FirstOrDefault();
            if (string.IsNullOrEmpty(isNotNullParameter))
            {
                return document;
            }

            var parameters = new SeparatedSyntaxList<ArgumentSyntax>().AddRange(
                new ArgumentSyntax[] 
                { 
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(isNotNullParameter))
                });
            
            var throwIfNullInvocation = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("ArgumentNullException.ThrowIfNull"))
                .WithArgumentList(SyntaxFactory.ArgumentList().WithArguments(parameters));

            var updatedRoot = root.ReplaceNode(invocationExpressionSyntax, throwIfNullInvocation);

            return document.WithSyntaxRoot(updatedRoot);
        }


        private static string[] GetArgumentNames(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var argumentList = invocationExpressionSyntax.ChildNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.ArgumentList)) as ArgumentListSyntax;
            if (argumentList is null)
            {
                return Array.Empty<string>();
            }

            var argumentNames = argumentList.Arguments.Select(x =>
            {
                if (x.DescendantNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.IdentifierName)) is IdentifierNameSyntax identifierNameSyntax)
                {
                    return identifierNameSyntax.Identifier.ValueText;
                }

                return string.Empty;
            }).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return argumentNames;
        }
    }
}
