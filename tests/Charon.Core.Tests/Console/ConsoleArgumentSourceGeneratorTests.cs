using System.Collections.Immutable;
using Charon.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Charon.Core.Tests.Console;

public sealed class ConsoleArgumentSourceGeneratorTests
{
    [Fact]
    public void Generator_GeneratesPostInitializationOutput_ArgumentHandlingAttribute()
    {
        const string userSource = """
            namespace TestApp;
            public class Dummy {}
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        var attrFile = Assert.Single(outputFiles, f => f.HintName == "ArgumentHandlingAttribute.g.cs");
        Assert.DoesNotContain(outputFiles, f => f.HintName == "ArgumentHandlerAttribute.g.cs");
        Assert.Contains("public sealed class ArgumentHandlingAttribute : Attribute", attrFile.SourceText);
        Assert.Contains("public string? Description { get; }", attrFile.SourceText);
        Assert.Contains("public bool CaseSensitive { get; set; }", attrFile.SourceText);
        Assert.Contains("public bool AllowUnknownArguments { get; set; }", attrFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForStaticPartialClassWithAttribute()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandlingAttribute]
            public static partial class ArgumentHandling
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "ArgumentHandlingAttribute.g.cs");
        var handlerAttrFile = Assert.Single(outputFiles, f => f.HintName == "ArgumentHandlerAttribute.g.cs");
        Assert.Contains("public sealed class ArgumentHandlerAttribute(string? name, string description) : Attribute", handlerAttrFile.SourceText);
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.ArgumentHandling.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.ArgumentHandling.Sample.g.cs");
        Assert.Contains("namespace TestApp;", sampleFile.SourceText);
        Assert.Contains("public static partial class ArgumentHandling", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
        Assert.Contains("global::System.Console.WriteLine(\"Sample\");", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForAttributeWithoutAttributeSuffix()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public static partial class CliArguments
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.CliArguments.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.CliArguments.Sample.g.cs");
        Assert.Contains("public static partial class CliArguments", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForFullyQualifiedAttribute()
    {
        const string userSource = """
            namespace TestApp;

            [Charon.Console.ArgumentHandlingAttribute("CLI Parser", CaseSensitive = true)]
            public static partial class FullyQualifiedArgs
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.FullyQualifiedArgs.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.FullyQualifiedArgs.Sample.g.cs");
        Assert.Contains("public static partial class FullyQualifiedArgs", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForInternalStaticPartialClass()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            internal static partial class InternalHandler
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.InternalHandler.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.InternalHandler.Sample.g.cs");
        Assert.Contains("internal static partial class InternalHandler", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForClassInGlobalNamespace()
    {
        const string userSource = """
            using Charon.Console;

            [ArgumentHandling]
            public static partial class GlobalArgumentHandling
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "GlobalArgumentHandling.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "GlobalArgumentHandling.Sample.g.cs");
        Assert.DoesNotContain("namespace ", sampleFile.SourceText);
        Assert.Contains("public static partial class GlobalArgumentHandling", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForGenericStaticPartialClass()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public static partial class GenericHandler<T>
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.GenericHandler.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.GenericHandler.Sample.g.cs");
        Assert.Contains("public static partial class GenericHandler<T>", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForMultipleTypeParameters()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public static partial class MultiGenericHandler<TKey, TValue>
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.MultiGenericHandler.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.MultiGenericHandler.Sample.g.cs");
        Assert.Contains("public static partial class MultiGenericHandler<TKey, TValue>", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesForMultipleCandidateClasses()
    {
        const string userSource1 = """
            using Charon.Console;

            namespace App1;

            [ArgumentHandling]
            public static partial class HandlerOne
            {
            }
            """;

        const string userSource2 = """
            using Charon.Console;

            namespace App2;

            [ArgumentHandling]
            public static partial class HandlerTwo
            {
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource1, userSource2);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "App1.HandlerOne.Sample.g.cs");
        Assert.Contains(outputFiles, f => f.HintName == "App2.HandlerTwo.Sample.g.cs");
    }

    [Fact]
    public void Generator_DeduplicatesPartialDeclarationsOfSameClass()
    {
        const string file1 = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public static partial class SplitHandler
            {
                public static void PartOne() {}
            }
            """;

        const string file2 = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public static partial class SplitHandler
            {
                public static void PartTwo() {}
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(file1, file2);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        // CS0579 is the C# compiler error for duplicate attribute on partial parts, but the generator deduplicates without error
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error && d.Id != "CS0579"));
        Assert.Single(outputFiles, f => f.HintName == "TestApp.SplitHandler.Sample.g.cs");
    }

    [Fact]
    public void Generator_IgnoresNonStaticClass()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandlingAttribute]
            public partial class NonStaticClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("NonStaticClass"));
    }

    [Fact]
    public void Generator_IgnoresNonPartialClass()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandlingAttribute]
            public static class NonPartialClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("NonPartialClass"));
    }

    [Fact]
    public void Generator_IgnoresClassWithoutAttribute()
    {
        const string userSource = """
            namespace TestApp;

            public static partial class NoAttributeClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("NoAttributeClass"));
    }

    [Fact]
    public void Generator_IgnoresClassWithUnrelatedAttribute()
    {
        const string userSource = """
            using System;

            namespace TestApp;

            [Obsolete]
            public static partial class UnrelatedAttributeClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("UnrelatedAttributeClass"));
    }

    [Fact]
    public void Generator_IgnoresStructInterfaceAndEnum()
    {
        const string userSource = """
            using Charon.Console;

            namespace TestApp;

            [ArgumentHandling]
            public struct StructCandidate {}

            [ArgumentHandling]
            public interface IInterfaceCandidate {}

            [ArgumentHandling]
            public enum EnumCandidate { Value }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("Candidate"));
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<(string HintName, string SourceText)> OutputFiles, Compilation OutputCompilation) RunGenerator(params string[] sources)
    {
        var syntaxTrees = sources.Select(s => CSharpSyntaxTree.ParseText(s)).ToArray();
        var assemblies = global::System.AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .ToHashSet();

        assemblies.Add(typeof(object).Assembly);
        assemblies.Add(typeof(global::System.Console).Assembly);
        assemblies.Add(typeof(ConsoleArgumentSourceGenerator).Assembly);

        var references = assemblies
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new ConsoleArgumentSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var runResult = driver.GetRunResult();
        var generatedFiles = runResult.GeneratedTrees
            .Select(t => (
                HintName: global::System.IO.Path.GetFileName(t.FilePath),
                SourceText: t.GetText().ToString()))
            .ToImmutableArray();

        return (diagnostics, generatedFiles, outputCompilation);
    }
}
