﻿namespace CatenaLogic.Analyzers.Tests
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
    internal class Program
    {
        public Program()
        {

        }

        public async Task MakeError()
        {
            ↓throw new InvalidOperationException(""Some invalid operation"", new Exception(""This is error!""));
        }
    }
}";
                var after = @"
namespace ConsoleApp1
{
    using Catel.Logging;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public Program()
        {

        }

        public async Task MakeError()
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(""Some invalid operation"", new Exception(""This is error!""));
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }
        }
    }
}
