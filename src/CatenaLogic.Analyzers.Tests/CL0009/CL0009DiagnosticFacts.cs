﻿namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0009DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0009_StringEmptyIsRecommended);

            [TestCase]
            public void InvalidCode_Assignment_LocalVariable()
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

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public void InvalidCode_Assignment_Field()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {        
        private readonly string foo1 = ↓"""";

        public Program(object arg)
        {
            Console.WriteLine(foo);
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reports_NoDiagnostic
        {
            [TestCase]
            public void ValidCode_CompileTime_Constant_Parameter()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {        
        public Program(object arg)
        {
            Write(arg.ToString());
        }

        private void Write(string message = """")
        {
            Console.WriteLine(message);
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }

            [TestCase]
            public void ValidCode_CompileTime_Constant_Attribute_Argument_1()
            {
                var before = @"
namespace ConsoleApp1
{
    using System.ComponentModel;

    public class ExtensionsCommandLineContext
    {
        [DisplayName(displayName: """")]
        public bool NoExtensions { get; set; }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }

            [TestCase]
            public void ValidCode_CompileTime_Constant_Attribute_Argument_2()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;
    using System.ComponentModel;
    using NUnit.Framework;

    [TestFixture]
    public class SomeTestFixture
    {
        [TestCase(null)]
        [TestCase("""")]
        [TestCase("" "")]
        public void ThrowsArgumentExceptionForNullOrEmptyType(string type)
        {
            Assert.Throws<ArgumentException>(() => throw new ArgumentException(type));
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }

            [TestCase]
            public void ValidCode_CompileTime_Constant_Field()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {        
        private const string foo1 = """";

        public Program(object arg)
        {
            Console.WriteLine(foo);
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }

            [TestCase]
            public void ValidCode_CompileTime_Constant_SwitchExpression()
            {
                var before = @"
namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case ""foo"":
                    break;
                case """":
                    break;
                default:
                    break;
            }
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }

            [TestCase]
            public void ValidCode_CompileTime_Constant_Attribute_Argument_Initializer()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;
    using System.ComponentModel;
    using NUnit.Framework;

    [TestFixture]
    public class SomeTestFixture
    {
        [TestCase(new[] { """" })]
        public void ThrowsArgumentExceptionForNullOrEmptyType(string[] types)
        {
            throw new ArgumentException(types);
        }
    }
}";

                Solution.Verify<VariablesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0009_StringEmptyIsRecommended, before));
            }
        }
    }
}
