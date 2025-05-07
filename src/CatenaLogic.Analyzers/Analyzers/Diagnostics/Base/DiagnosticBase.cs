namespace CatenaLogic.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public abstract class DiagnosticBase : IDiagnostic
    {
        private static readonly Dictionary<string, bool> IsTestProjectCache = new Dictionary<string, bool>();

        public virtual void HandleOperation(OperationAnalysisContext context)
        {
        }

        public virtual void HandleSymbol(SymbolAnalysisContext context)
        {
        }

        public virtual void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
        }

        protected virtual bool IsTestProject(SyntaxNodeAnalysisContext context)
        {
            var assemblyName = context.Compilation.Assembly.Name;

            if (string.IsNullOrWhiteSpace(assemblyName) ||
                string.Equals(assemblyName, "Unknown", StringComparison.OrdinalIgnoreCase))
            {
                // cannot cache
                return IsTestProject(context.Options, context.Compilation);
            }

            if (!IsTestProjectCache.TryGetValue(assemblyName, out var isTestProject))
            {
                // cache miss
                isTestProject = IsTestProject(context.Options, context.Compilation); 
                
                IsTestProjectCache[assemblyName] = isTestProject;
            }

            return isTestProject;
        }

        private bool IsTestProject(AnalyzerOptions analyzerOptions, Compilation compilation)
        {
            // Check 1: msbuild property
            if (analyzerOptions.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue($"build_property.IsTestProject", out var isTestProjectStringValue))
            {
                if (bool.TryParse(isTestProjectStringValue, out var isTestProjectValue) && isTestProjectValue)
                {
                    return true;
                }
            }

            // Check 2: project name (BIG assumption)
            if (compilation.Assembly.Name.EndsWith(".Tests", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Check 3: referenced assembly
            var referencedAssemblyNames = compilation.ReferencedAssemblyNames;
            if (referencedAssemblyNames.Any(x => string.Equals(x.Name, "Microsoft.Testing.Platform", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }

#if DEBUG
        private void AttachDebugger()
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
        }
#endif
    }
}
