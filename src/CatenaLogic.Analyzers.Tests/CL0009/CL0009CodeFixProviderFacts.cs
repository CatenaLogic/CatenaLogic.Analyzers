﻿namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0009CodeFixProviderFacts
    {
        public class CanFix
        {
            private static readonly CodeFixProvider Fixer = new CL0009CodeFixProvider();

            [TestCase]
            public void InvalidCode_Default()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        public Program(object arg)
        {
            var foo = ↓"""";

            Console.WriteLine(foo);
        }
    }
}";
                var after = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        public Program(object arg)
        {
            var foo = string.Empty;

            Console.WriteLine(foo);
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }

            [TestCase]
            public void InvalidCode_NestedInvocation()
            {
                var before = @"
namespace ConsoleApp1
{
    using System.ComponentModel;
    using NUnit.Framework;

    public class DummyClass
    {
        [TestCase]
        public void Convert_EmptyString()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert(↓"""", typeof (bool), null, (CultureInfo)null));
        }
    }
}";

                var after = @"
namespace ConsoleApp1
{
    using System.ComponentModel;
    using NUnit.Framework;

    public class DummyClass
    {
        [TestCase]
        public void Convert_EmptyString()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert(string.Empty, typeof (bool), null, (CultureInfo)null));
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.CodeFix(analyzer, Fixer, before, after));
            }
        }
    }
}
