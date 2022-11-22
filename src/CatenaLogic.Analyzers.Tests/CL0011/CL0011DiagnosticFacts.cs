﻿namespace CatenaLogic.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0011DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0011_ProvideCatelLogOnThrowingException);

            [TestCase]
            public async Task InvalidCode_Exception_Thrown()
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

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reposts_NoDiagnostic
        {
            [TestCase]
            public async Task ValidCode_NoMessageProvided()
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
            throw new InvalidOperationException();
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0011_ProvideCatelLogOnThrowingException, before));
            }

            [TestCase]
            public async Task ValidCode_Log_ErrorAndCreateException()
            {
                var before = @"
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
            throw Log.ErrorAndCreateException<InvalidOperationException>(""Some invalid operation"");
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0011_ProvideCatelLogOnThrowingException, before));
            }

            [TestCase]
            public async Task ValidCode_Manually_Created_Exception_NotThrown()
            {
                var before = @"
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
            var exception = CreatesException();
            Console.WriteLine(exception);
        }

        public Exception CreatesException()
        {
            return new InvalidOperationException(""Some invalid operation"");
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0011_ProvideCatelLogOnThrowingException, before));
            }

            [TestCase]
            public async Task ValidCode_Exception_Created_AsCallback()
            {
                var before = @"
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
            throw Log.ErrorAndCreateException<InvalidOperationException>(text => new InvalidOperationException(text), ""This is message format"");
        }
    }
}";

                Solution.Verify<ExceptionsAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0011_ProvideCatelLogOnThrowingException, before));
            }
        }
    }
}
