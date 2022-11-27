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

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public void InvalidCode_Contains_MembersRegions_Nested()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        ↓#region Fields
        #region Foo
        #region FooFoo
        private readonly string foo1 = string.Empty;
        #endregion
        #endregion
        ↓#endregion

        #region ctor_wrap
        ↓#region Constructors
        #region default
        public Program(object arg)
        {
            Console.WriteLine(foo1);
        }
        #endregion
        ↓#endregion
        #endregion
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reports_NoDiagnostic
        {
            [TestCase]
            public void ValidCode_Contains_CustomRegions()
            {
                var before = @"
namespace ConsoleApp1
{
    using System;

    internal class Program : IFoo, IFoo2
    {
        #region IFoo
        public void Foo2()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IFoo2
        void IFoo.Foo()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}";

                Solution.Verify<ClassesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0010_DontKeepClassMemberRegions, before));
            }
        }
    }
}
