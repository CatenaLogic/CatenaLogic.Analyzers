; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md


## Release 1.3.3

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
CL0001  | CatenaLogic.Analyzers.Async   |  Warning | Use async overload inside this async method, [Documentation](https://github.com/CatenaLogic/CatenaLogic.Analyzers/blob/develop/doc/CL0001.md)
CL0002  | CatenaLogic.Analyzers.Async | Warning | Use "Async" suffix for async methods
CL0003  | CatenaLogic.Analyzers.Namespace | Warning | Don't use "Extensions" namespace for classes containing Extensions methods
CL0004  | CatenaLogic.Analyzers.Namespace | Warning | Don't use "Interfaces" namespace for interfaces
CL0005  | CatenaLogic.Analyzers.Namespace | Warning | Don't use ""Helpers"" namespace for helper classes
CL0006  | CatenaLogic.Analyzers.Expression | Warning | Using "is" statement inside null comparison expression is recommended style
CL0007  | CatenaLogic.Analyzers.Text | Warning | Don't place header on top of code file
CL0009  | CatenaLogic.Analyzers.Variable | Warning | Use string.Empty
CL0010 | CatenaLogic.Analyzers.Text | Disabled | Descriptors