namespace CatenaLogic.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Removes common class member regions created by Resharper.
    /// Disabled by default because of performance cost of looking through all class descendants
    /// </summary>
    internal class CL0010Diagnostic : DiagnosticBase
    {
        public const string Id = "CL0010";

        private static ImmutableArray<string> RegionsToRemove =>
            ImmutableArray.Create(
                "constructors", "ctor", "ctors", "constructor",
                "constants", "constant",
                "methods", "method",
                "fields", "field",
                "events", "event",
                "properties", "property");

        public override void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classSyntax)
            {
                return;
            }

            var regionTrivias = classSyntax.DescendantTrivia(child => true, true)
                .Where(x => x.IsKind(SyntaxKind.RegionDirectiveTrivia) || x.IsKind(SyntaxKind.EndRegionDirectiveTrivia)).ToImmutableArray();

            if (regionTrivias.Length == 0)
            {
                return;
            }

            var regionPairs = new List<(SyntaxTrivia, SyntaxTrivia)>();

            // Walk through directives, support nesting
            var walkStack = new Stack<SyntaxTrivia>();
            foreach (var trivia in regionTrivias)
            {
                if (trivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
                {
                    walkStack.Push(trivia);
                    continue;
                }

                if (trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                {
                    if (walkStack.Count == 0)
                    {
                        // Broken Syntax, skip all
                        return;
                    }

                    var beginOfCurrentRegion = walkStack.Pop();
                    var regionName = GetRegionNameFromSyntaxTrivia(beginOfCurrentRegion)?.ToLowerInvariant() ?? string.Empty;
                    if (string.IsNullOrEmpty(regionName) || !RegionsToRemove.Contains(regionName))
                    {
                        continue;
                    }

                    regionPairs.Add((beginOfCurrentRegion, trivia));
                }
            }

            var listOfLocations = new List<Location>();
            if (!regionPairs.Any())
            {
                return;
            }

            // Report diagnostics:
            foreach (var pair in regionPairs)
            {
                var beginRegionLocation = pair.Item1.GetLocation();
                var endRegionLocation = pair.Item2.GetLocation();

                listOfLocations.Add(beginRegionLocation);
                listOfLocations.Add(endRegionLocation);
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.CL0010_DontKeepClassMemberRegions, context.Node.GetLocation(), additionalLocations: listOfLocations));
        }

        private string? GetRegionNameFromSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            var node = syntaxTrivia.GetStructure();
            if (node is null)
            {
                return null;
            }

            var endTokenWithNameTrivia = node.ChildTokens().FirstOrDefault(x => x.IsKind(SyntaxKind.EndOfDirectiveToken));

            var regionName = endTokenWithNameTrivia.LeadingTrivia.ToFullString();
            return regionName;
        }
    }
}
