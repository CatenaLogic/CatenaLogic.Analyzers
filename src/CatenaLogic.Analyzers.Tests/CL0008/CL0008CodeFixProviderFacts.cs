namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0008CodeFixProviderFacts
    {
        public class CanFix
        {
            private static readonly CodeFixProvider Fixer = new CL0008CodeFixProvider();

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

                Solution.Verify<ArgumentsAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }
        }
    }
}
