namespace CatenaLogic.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0006AnalyzerUnitTests
    {
        private static readonly ExpressionsAnalyzer Analyzer = new ExpressionsAnalyzer();

        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0006_ConstantPatternIsRecommendedForNullCheck);

        [Test]
        public void Invalid_Code_01()
        {
            var before = @"
namespace CatenaLogic.Analyzers.Extensions
{
public class C
{
        private static readonly int? PossibleNullVar = 3;

        public bool IsNullCheck { get; set; } = PossibleNullVar ↓== null;

        public void MyTestMethod()
        {
            int? foo = 3;
            var isFooNull = foo ↓== null;
            var isFooNotNull = foo ↓!= null;
        }
}
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }
    }

    public class C
    {
        private static readonly int? PossibleNullVar = 3;

        public bool IsNullCheck { get; set; } = PossibleNullVar == null;

        public void MyTestMethod()
        {
            int? foo = 3;
            var isFooNull = foo == null;
            var isFooNotNull = foo != null;
        }
    }
}
