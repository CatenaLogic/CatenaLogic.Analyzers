namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class DiagnosticRuleBase : IDiagnostic
    {
        public virtual void HandleOperation(OperationAnalysisContext context)
        {
        }

        public virtual void HandleSymbol(SymbolAnalysisContext context)
        {
        }

        public virtual void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
        }
    }
}
