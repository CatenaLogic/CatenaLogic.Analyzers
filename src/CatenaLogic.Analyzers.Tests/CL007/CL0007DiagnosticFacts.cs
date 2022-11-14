namespace CatenaLogic.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0007DiagnosticFacts
    {
        public class Reports_Diagnostic_CL0007
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile);

            [Test]
            public async Task Invalid_Code_MultilineComment_Multiplelines()
            {
                var before = @"
/* --------------------------------------------------------------------------------------------------------------------
   <copyright file=""DummyLocator.cs"" company=""Wildgums development team"">
    Copyright (c) 2008 - 2022 Wildgums development team. All rights reserved.
   </copyright>
 --------------------------------------------------------------------------------------------------------------------*/

namespace Mock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Default implementation of the <see cref=""IServiceLocator""/> interface.
    /// </summary>
    public class DummyLocator : IServiceLocator
    {
    }
}";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [TestCase]
            public async Task Invalid_Code_SingleComment_Multiplelines()
            {
                var before = @"
// --------------------------------------------------------------------------------------------------------------------
// <copyright file=""HttpClientExtensions.system.cs"" company=""CatenaLogic"">
//   Copyright (c) 2008 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace DummyClass
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Catel;

    internal static partial class HttpClientExtensions
    {
        #region Methods
        public static void AcceptGzipAndDeflate(this HttpClient httpClient)
        {
            Argument.IsNotNull(() => httpClient);

            httpClient.AcceptGzip();
            httpClient.AcceptDeflate();
        }

        public static void AcceptGzip(this HttpClient httpClient)
        {
            Argument.IsNotNull(() => httpClient);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(""gzip""));
        }

        public static void AcceptDeflate(this HttpClient httpClient)
        {
            Argument.IsNotNull(() => httpClient);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(""deflate""));
        }
        #endregion
    }
}";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }

            [Test]
            public async Task Invalid_Code_SingleComment_Multiplelines_Has_Trailing_EmptyLine()
            {
                var before = @"
// --------------------------------------------------------------------------------------------------------------------
// <copyright file=""DummyLocator.cs"" company=""Wildgums development team"">
//   Copyright (c) 2008 - 2022 Wildgums development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Default implementation of the <see cref=""IServiceLocator""/> interface.
    /// </summary>
    public class DummyLocator : IServiceLocator
    {
    }
}
";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reports_NoDiagnostic
        {
            [TestCase]
            public async Task ValidCode_NoLeadingComments()
            {
                var before =
@"namespace Mock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Default implementation of the <see cref=""IServiceLocator""/> interface.
    /// </summary>
    public class DummyLocator : IServiceLocator
    {
    }
}";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, before));
            }

            [TestCase]
            public async Task ValidCode_SingleTextLine_AnyEmptyLines()
            {
                var before =
@"//#define CHECK_LICENSE




namespace Mock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Default implementation of the <see cref=""IServiceLocator""/> interface.
    /// </summary>
    public class DummyLocator : IServiceLocator
    {
    }
}";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, before));
            }

            [Test]
            public async Task Invalid_Code_MultilineComment_Multiplelines()
            {
                var before = @"
/* --------------------------------------------------------------------------------------------------------------------
    file=""DummyLocator.cs"" company=""Wildgums development team""
    Copyright (c) 2008 - 2022 Wildgums development team. All rights reserved.
 --------------------------------------------------------------------------------------------------------------------*/

namespace Mock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Default implementation of the <see cref=""IServiceLocator""/> interface.
    /// </summary>
    public class DummyLocator : IServiceLocator
    {
    }
}";

                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.NoAnalyzerDiagnostics(analyzer, Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile, before));
            }
        }
    }
}
