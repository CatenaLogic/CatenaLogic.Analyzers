namespace CatenaLogic.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture]
    public class CL0007AnalyzerFacts
    {
        public class Reports_Diagnostic_CL0007
        {
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.CL0007_DontPlaceHeaderOnTopOfCodeFile);

            [Test]
            public async Task Invalid_Code_SingleComment_Multiplelines()
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
}
";
                Solution.Verify<NamespacesAnalyzer>(analyzer => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic, before));
            }
        }

        public class Reports_NoDiagnostic
        {

        }
    }
}
