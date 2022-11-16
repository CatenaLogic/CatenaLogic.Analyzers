namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0011CodeFixProviderFacts
    {
        public class CanFix
        {
            private static readonly CodeFixProvider Fixer = new CL0011CodeFixProvider();

            [TestCase]
            public void InvalidCode_Default()
            {
                var before = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        public Program(object arg)
        {
            ↓Argument.IsNotNull(() => arg);
        }
    }
}";
                var after = @"
namespace ConsoleApp1
{
    using Catel;

    internal class Program
    {
        public Program(object arg)
        {
            ArgumentNullException.ThrowIfNull(arg);
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }
        }
    }
}
