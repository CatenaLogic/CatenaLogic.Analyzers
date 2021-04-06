namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0003AnalyzerUnitTests
    {
        private static readonly NamespacesAnalyzer Analyzer = new NamespacesAnalyzer();

        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0003_DontUseExtensionsNamespace);

        [Test]
        public void Invalid_Code_01()
        {
            var before = @"
namespace CatenaLogic.Analyzers.↓Extensions
{
using System.IO;
using System.Threading.Tasks;

public class C
{
    public async Task MyMethod()
    {
        using (var fileStream = File.OpenRead(""filename""))
        {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        // Long namespaces
        public void Invalid_Code_02()
        {
            var before = @"
namespace CatenaLogic.Analyzers.Analyzers2.Analyzers3.Analyzers4.↓Extensions
{
using System.IO;
using System.Threading.Tasks;

public class C
{
    public async Task MyMethod()
    {
        using (var fileStream = File.OpenRead(""filename""))
        {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        // Not-nested name
        public void Invalid_Code_03()
        {
            var before = @"
namespace ↓Extension
{
using System.IO;
using System.Threading.Tasks;

public class C
{
    public async Task MyMethod()
    {
        using (var fileStream = File.OpenRead(""filename""))
        {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        // Not-nested name
        public void Valid_Code_01()
        {
            var before = @"
namespace ExtendedAnalyzers
{
using System.IO;
using System.Threading.Tasks;

public class C
{
    public async Task MyMethod()
    {
        using (var fileStream = File.OpenRead(""filename""))
        {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }
}";
            RoslynAssert.Valid(Analyzer, before);
        }
    }
}
