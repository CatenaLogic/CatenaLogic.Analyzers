namespace CatenaLogic.Analyzers.Tests
{
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

        [Test]
        public void Valid_Code_01()
        {
            var before = @"
using System.Collections.Generic;
using System.Linq;

public class C
{
    public IEnumerable<string> MyMethod()
    {
        var list = new List<string>().AsQueryable();

        return from t in list
               where t == null
               select t;
    }
}";

            RoslynAssert.Valid(Analyzer, before);
        }

        [Test]
        public void Valid_Code_02()
        {
            var before = @"
using System.Collections.Generic;
using System.Linq;

public class C
{
    public IEnumerable<string> MyMethod()
    {
        var list = new List<string>().AsQueryable();

        return list.Where(x => x == null || x != null).ToList();
    }
}";

            RoslynAssert.Valid(Analyzer, before);
        }
    }
}
