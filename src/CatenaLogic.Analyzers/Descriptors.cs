namespace CatenaLogic.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        internal static readonly DiagnosticDescriptor CL0001_UseAsyncOverloadInsideAsyncMethods = Create(
            id: "CL0001",
            title: "Use async overload inside this async method",
            messageFormat: "This method has an async overload. Since this method is invoked from within an async method, it's recommended to use the async overload.",
            category: AnalyzerCategory.Async,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "This method has an async overload. Since this method is invoked from within an async method, it's recommended to use the async overload.");

        internal static readonly DiagnosticDescriptor CL0002_UseAsyncSuffixForAsyncMethods = Create(
            id: "CL0002",
            title: @"Use ""Async"" suffix for async methods",
            messageFormat: @"This method marked with async keyword. The Microsoft's recommended naming convention assume to append ""Async"" to the names of methods that have an async modifier.",
            category: AnalyzerCategory.Async,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: @"This method marked with async keyword. The Microsoft's recommended naming convention assume to append ""Async"" to the names of methods that have an async modifier.");

        internal static readonly DiagnosticDescriptor CL0003_DontUseExtensionsNamespace = Create(
            id: "CL0003",
            title: @"Don't use ""Extensions"" namespace for classes containing Extensions methods",
            messageFormat: "This namespace should be renamed",
            category: AnalyzerCategory.Namespace,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: @"This namespace should be renamed.");

        internal static readonly DiagnosticDescriptor CL0004_DontUseInterfacesNamespace = Create(
           id: "CL0004",
           title: @"Don't use ""Interfaces"" namespace for interfaces",
           messageFormat: "This namespace should be renamed",
           category: AnalyzerCategory.Namespace,
           defaultSeverity: DiagnosticSeverity.Warning,
           isEnabledByDefault: true,
           description: @"This namespace should be renamed.");

        internal static readonly DiagnosticDescriptor CL0005_DontUseHelpersNamespace = Create(
           id: "CL0005",
           title: @"Don't use ""Helpers"" namespace for helper classes",
           messageFormat: "This namespace should be renamed",
           category: AnalyzerCategory.Namespace,
           defaultSeverity: DiagnosticSeverity.Warning,
           isEnabledByDefault: true,
           description: @"This namespace should be renamed.");

        internal static readonly DiagnosticDescriptor CL0006_ConstantPatternIsRecommendedForNullCheck = Create(
           id: "CL0006",
           title: @"Using ""is"" statement inside null comparison expression is recommended style",
           messageFormat: @"Starting with C# 7.0 the constant pattern is supported by the ""is"" keyword. Performing pattern matching with ""null"" ""is"" can be used to test whether an expression equals a null.",
           category: AnalyzerCategory.Expression,
           defaultSeverity: DiagnosticSeverity.Warning,
           isEnabledByDefault: true,
           description: @"Starting with C# 7.0 the constant pattern is supported by the ""is"" keyword. Performing pattern matching with ""null"" ""is"" can be used to test whether an expression equals a null.");

        internal static readonly DiagnosticDescriptor CL0007_DontPlaceHeaderOnTopOfCodeFile = Create(
            id: CL0007Diagnostic.Id,
            title: @"Don't place header on top of code file",
            messageFormat: @"Don't place header on top of code file",
            category: AnalyzerCategory.Text,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: @"Don't place header on top of code file.");

        internal static readonly DiagnosticDescriptor CL0009_StringEmptyIsRecommended = Create(
            id: CL0009Diagnostic.Id,
            title: "Use string.Empty",
            messageFormat: "Use string.Empty instead of empty string literal",
            category: AnalyzerCategory.Variable,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: @"Use string.Empty instead of empty string literal.");

        internal static readonly DiagnosticDescriptor CL0010_DontKeepClassMemberRegions = Create(
            id: CL0010Diagnostic.Id,
            title: "Don't keep class member regions",
            messageFormat: "Don't use #region for organizing class members",
            category: AnalyzerCategory.Text,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false,
            description: "Don't use #region for organizing class members.");

        /// <summary>
        /// Create a DiagnosticDescriptor, which provides description about a <see cref="T:Microsoft.CodeAnalysis.Diagnostic" />.
        /// NOTE: For localizable <paramref name="title" />, <paramref name="description" /> and/or <paramref name="messageFormat" />,
        /// use constructor overload <see cref="M:Microsoft.CodeAnalysis.DiagnosticDescriptor.#ctor(System.String,Microsoft.CodeAnalysis.LocalizableString,Microsoft.CodeAnalysis.LocalizableString,System.String,Microsoft.CodeAnalysis.DiagnosticSeverity,System.Boolean,Microsoft.CodeAnalysis.LocalizableString,System.String,System.String[])" />.
        /// </summary>
        /// <param name="id">A unique identifier for the diagnostic. For example, code analysis diagnostic ID "CA1001".</param>
        /// <param name="title">A short title describing the diagnostic. For example, for CA1001: "Types that own disposable fields should be disposable".</param>
        /// <param name="messageFormat">A format message string, which can be passed as the first argument to <see cref="M:System.String.Format(System.String,System.Object[])" /> when creating the diagnostic message with this descriptor.
        /// For example, for CA1001: "Implement IDisposable on '{0}' because it creates members of the following IDisposable types: '{1}'.</param>
        /// <param name="category">The category of the diagnostic (like Design, Naming etc.). For example, for CA1001: "Microsoft.Design".</param>
        /// <param name="defaultSeverity">Default severity of the diagnostic.</param>
        /// <param name="isEnabledByDefault">True if the diagnostic is enabled by default.</param>
        /// <param name="description">An optional longer description of the diagnostic.</param>
        /// <param name="customTags">Optional custom tags for the diagnostic. See <see cref="T:Microsoft.CodeAnalysis.WellKnownDiagnosticTags" /> for some well known tags.</param>
        private static DiagnosticDescriptor Create(
          string id,
          string title,
          string messageFormat,
          string category,
          DiagnosticSeverity defaultSeverity,
          bool isEnabledByDefault,
          string description = "",
          params string[] customTags)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: defaultSeverity,
                isEnabledByDefault: isEnabledByDefault,
                description: description,
                helpLinkUri: HelpLink.ForId(id),
                customTags: customTags);
        }
    }
}
