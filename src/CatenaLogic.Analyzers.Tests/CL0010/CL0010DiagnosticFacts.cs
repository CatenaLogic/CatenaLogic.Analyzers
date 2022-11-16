namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    internal class CL0010DiagnosticFacts
    {
        public class Reports_Diagnostic
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0010_DontKeepClassMemberRegions);

            [TestCase]
            public void InvalidCode_Contains_MembersRegions()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        #region Fields
        private readonly string foo1 = "";
        #endregion

        #region Events
        #endregion

        #region Constructors
        public Program(object arg)
        {

        }
        #endregion

        #region Properties
        public string Error { get; set; }
        #endregion

        #region Methods
        private void Write(string message = "")
        {
            Console.WriteLine("");
        }
        #endregion
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reposts_NoDiagnostic
        {
            [TestCase]
            public void ValidCode_Contains_CustomRegions()
            {

            }
        }
    }
}
