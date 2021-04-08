﻿namespace CatenaLogic.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0002AnalyzerUnitTests
    {
        private static readonly MethodsAnalyzer Analyzer = new MethodsAnalyzer();

        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0002_UseAsyncSuffixForAsyncMethods);

        [Test]
        public void Invalid_Code_01()
        {
            var before = @"
using System;
using System.IO;
using System.Threading.Tasks;

public class C
{
    public async Task ↓MyMethod()
    {
        using (var fileStream = File.OpenRead(""filename""))
        {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
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
    public async void MyMethod()
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
        // Ingore Main check
        public void Valid_Code_03()
        {
            var before = @"
using System;
using System.IO;
using System.Threading.Tasks;

class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(""Hello World!"");

            using (var fileStream = File.OpenRead(""filename""))
            {
                var reader = new StreamReader(fileStream);
                var text = await reader.ReadToEndAsync();
            }
        }
    }";

            RoslynAssert.Valid(Analyzer, before);
        }
    }
}
