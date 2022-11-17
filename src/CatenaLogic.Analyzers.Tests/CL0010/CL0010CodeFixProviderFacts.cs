namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0010CodeFixProviderFacts
    {
        public class CanFix
        {
            private static readonly CodeFixProvider Fixer = new CL0010CodeFixProvider();

            [TestCase]
            public void InvalidCode_Default()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        ↓#region Fields
        private readonly string foo1 = """";
        ↓#endregion

        ↓#region Events
        ↓#endregion

        ↓#region Constructors
        public Program(object arg)
        {
            Console.WriteLine(foo1);
            Error = string.Empty;
        }
        ↓#endregion

        ↓#region Properties
        public string Error { get; set; }
        ↓#endregion

        ↓#region Methods
        private void Write(string message)
        {
            Console.WriteLine(message);
        }
        ↓#endregion
    }
}";
                var after = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        private readonly string foo1 = """";


        public Program(object arg)
        {
            Console.WriteLine(foo1);
            Error = string.Empty;
        }

        public string Error { get; set; }

        private void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}"; ;

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }
        }
    }
}
