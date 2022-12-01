namespace CatenaLogic.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class TypeSyntaxExtensions
    {
        public static bool IsTask(this TypeSyntax typeSyntax, SyntaxNodeAnalysisContext context)
        {
            if (typeSyntax == KnownSymbols.Task)
            {
                return true;
            }

            if (typeSyntax.IsAssignableTo(KnownSymbols.Task, context.SemanticModel))
            {
                return true;
            }

            var genericNameSyntax = typeSyntax as GenericNameSyntax;
            if (genericNameSyntax is null == false)
            {
                var identifierName = genericNameSyntax.Identifier.Value as string;
                if (identifierName == "Task")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
