using System.Collections.Immutable;
using Charon.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Charon.Core.Tests.Types;

public sealed class ConsoleArgumentSourceGeneratorTests
{
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

        var (diagnostics, outputFiles) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "ArgumentHandlingAttribute.g.cs");
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.ArgumentHandling.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.ArgumentHandling.Sample.g.cs");
        Assert.Contains("public static partial class ArgumentHandling", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
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

        var (diagnostics, outputFiles) = RunGenerator(userSource);

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

        var (diagnostics, outputFiles) = RunGenerator(userSource);

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

        var (diagnostics, outputFiles) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.Contains("NoAttributeClass"));
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<(string HintName, string SourceText)> OutputFiles) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = global::System.AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            new[] { syntaxTree },
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

        return (diagnostics, generatedFiles);
    }
}
