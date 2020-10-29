namespace CatenaLogic.Analyzers.Tests
{
    using System.IO;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0001AnalyzerUnitTests
    {
        private static readonly MethodsAnalyzer Analyzer = new MethodsAnalyzer();

        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0001_UseAsyncOverloadInsideAsyncMethods);

        [Test]
        public void Invalid_Code_01()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public async Task MyMethodAsync()
        {
            using (var fileStream = File.OpenRead(""filename""))
            {
                var reader = new StreamReader(fileStream);
                var text = ↓reader.ReadToEnd();
            }
        }
    }";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        public void Invalid_Code_02()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public async Task MyMethodAsync()
        {
            // Note: I know it's stupid to return a task inside a using, but this is just for testing purposes
            using (var fileStream = File.OpenWrite(""filename""))
            {
                 ↓fileStream.Write(Array.Empty<byte>(), 0, 0);
            }
        }
    }";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        public void Invalid_Code_03()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public async Task<bool> MyMethodAsync()
        {
            // Note: I know it's stupid to return a task inside a using, but this is just for testing purposes
            using (var fileStream = File.OpenWrite(""filename""))
            {
                 ↓fileStream.Write(Array.Empty<byte>(), 0, 0);

                 return true;
            }
        }
    }";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, before);
        }

        [Test]
        public void Valid_Code_01()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public async Task MyMethodAsync()
        {
            using (var fileStream = File.OpenRead(""filename""))
            {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }";

            RoslynAssert.Valid(Analyzer, before);
        }

        [Test]
        public void Valid_Code_02()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public Task MyMethodAsync()
        {
            // Note: I know it's stupid to return a task inside a using, but this is just for testing purposes
            using (var fileStream = File.OpenRead(""filename""))
            {
                var reader = new StreamReader(fileStream);
                return reader.ReadToEndAsync();
            }
        }
    }";

            RoslynAssert.Valid(Analyzer, before);
        }


        [Test]
        public void Valid_Code_03()
        {
            var before = @"
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class C 
    {
        public Task MyMethodAsync()
        {
            // Note: I know it's stupid to return a task inside a using, but this is just for testing purposes
            using (var fileStream = File.OpenWrite(""filename""))
            {
                return fileStream.WriteAsync(Array.Empty<byte>(), 0, 0);
            }
        }
    }";

            RoslynAssert.Valid(Analyzer, before);
        }

        [Test]
        public void Valid_NoCode()
        {
            var before = @"";

            RoslynAssert.Valid(Analyzer, before);
        }
    }
}
