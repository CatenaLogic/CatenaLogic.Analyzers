﻿using Gu.Roslyn.Asserts;

[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: TransitiveMetadataReferences(
    typeof(CatenaLogic.Analyzers.Tests.ValidCodeWithAllAnalyzers)
    //typeof(Catel.MVVMModule))]
    )]
