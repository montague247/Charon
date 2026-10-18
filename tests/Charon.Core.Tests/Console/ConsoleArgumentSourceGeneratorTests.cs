using System.Collections.Immutable;
using Charon.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Charon.Core.Tests.Console;

public sealed class ConsoleArgumentSourceGeneratorTests
{
    [Fact]
    public void Generator_GeneratesPostInitializationOutput_ArgumentProcessorAttribute()
    {
        const string userSource = """
            namespace TestApp;
            public class Dummy {}
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        var attrFile = Assert.Single(outputFiles, f => f.HintName == "ArgumentProcessorAttribute.g.cs");
        Assert.DoesNotContain(outputFiles, f => f.HintName == "ArgumentHandlerAttribute.g.cs");
        Assert.DoesNotContain(outputFiles, f => f.HintName == "IArgumentHandler.g.cs");
        Assert.DoesNotContain(outputFiles, f => f.HintName == "IGlobalOptions.g.cs");
        Assert.Contains("[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]", attrFile.SourceText);
        Assert.Contains("public sealed class ArgumentProcessorAttribute : Attribute", attrFile.SourceText);
        Assert.Contains("public string? Description { get; }", attrFile.SourceText);
        Assert.Contains("public string Name { get; } = \"ArgumentProcessor\";", attrFile.SourceText);
        Assert.Contains("public bool CaseSensitive { get; set; }", attrFile.SourceText);
        Assert.Contains("public bool AllowUnknownArguments { get; set; }", attrFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForAssemblyWithAttribute()
    {
        const string userSource = """
            using Charon.Console;

            [assembly: ArgumentProcessorAttribute]
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "ArgumentProcessorAttribute.g.cs");
        var handlerAttrFile = Assert.Single(outputFiles, f => f.HintName == "ArgumentHandlerAttribute.g.cs");
        Assert.Contains("public sealed class ArgumentHandlerAttribute(string? name, string description) : Attribute", handlerAttrFile.SourceText);
        Assert.Single(outputFiles, f => f.HintName == "IArgumentHandler.g.cs");
        Assert.Single(outputFiles, f => f.HintName == "IGlobalOptions.g.cs");
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.ArgumentProcessor.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.ArgumentProcessor.Sample.g.cs");
        Assert.Contains("namespace TestApp;", sampleFile.SourceText);
        Assert.Contains("public static partial class ArgumentProcessor", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
        Assert.Contains("global::System.Console.WriteLine(\"Sample\");", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForAttributeWithoutAttributeSuffix()
    {
        const string userSource = """
            using Charon.Console;

            [assembly: ArgumentProcessor]
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.ArgumentProcessor.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.ArgumentProcessor.Sample.g.cs");
        Assert.Contains("public static partial class ArgumentProcessor", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForCustomName()
    {
        const string userSource = """
            using Charon.Console;

            [assembly: ArgumentProcessor("CLI Parser", "CliArguments")]
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "TestApp.CliArguments.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "TestApp.CliArguments.Sample.g.cs");
        Assert.Contains("namespace TestApp;", sampleFile.SourceText);
        Assert.Contains("public static partial class CliArguments", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_GeneratesSampleMethod_ForFullyQualifiedAttribute()
    {
        const string userSource = """
            [assembly: Charon.Console.ArgumentProcessorAttribute("CLI Parser", "FullyQualifiedArgs", CaseSensitive = true)]
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
    public void Generator_GeneratesSampleMethod_ForGlobalNamespaceWhenAssemblyNameIsNull()
    {
        const string userSource = """
            using Charon.Console;

            [assembly: ArgumentProcessor("CLI Parser", "GlobalArgumentProcessor")]
            """;

        var (diagnostics, outputFiles, compilation) = RunGeneratorWithAssemblyName(null, userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains(outputFiles, f => f.HintName == "GlobalArgumentProcessor.Sample.g.cs");

        var sampleFile = outputFiles.First(f => f.HintName == "GlobalArgumentProcessor.Sample.g.cs");
        Assert.DoesNotContain("namespace ", sampleFile.SourceText);
        Assert.Contains("public static partial class GlobalArgumentProcessor", sampleFile.SourceText);
        Assert.Contains("public static void Sample()", sampleFile.SourceText);
    }

    [Fact]
    public void Generator_MergesWithPartialClassDeclaration()
    {
        const string file1 = """
            using Charon.Console;

            [assembly: ArgumentProcessor("CLI Parser", "SplitHandler")]

            namespace TestApp;

            public static partial class SplitHandler
            {
                public static void PartOne() {}
            }
            """;

        const string file2 = """
            namespace TestApp;

            public static partial class SplitHandler
            {
                public static void PartTwo() => Sample();
            }
            """;

        var (diagnostics, outputFiles, compilation) = RunGenerator(file1, file2);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Single(outputFiles, f => f.HintName == "TestApp.SplitHandler.Sample.g.cs");
    }

    [Fact]
    public void Generator_IgnoresAssemblyWithoutAttribute()
    {
        const string userSource = """
            namespace TestApp;

            public static partial class NoAttributeClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.EndsWith(".Sample.g.cs"));
    }

    [Fact]
    public void Generator_IgnoresAssemblyWithUnrelatedAttribute()
    {
        const string userSource = """
            using System.Reflection;

            [assembly: AssemblyDescription("Unrelated")]

            namespace TestApp;

            public static partial class UnrelatedAttributeClass
            {
            }
            """;

        var (diagnostics, outputFiles, _) = RunGenerator(userSource);

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.DoesNotContain(outputFiles, f => f.HintName.EndsWith(".Sample.g.cs"));
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<(string HintName, string SourceText)> OutputFiles, Compilation OutputCompilation) RunGenerator(params string[] sources)
        => RunGeneratorWithAssemblyName("TestApp", sources);

    private static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<(string HintName, string SourceText)> OutputFiles, Compilation OutputCompilation) RunGeneratorWithAssemblyName(string? assemblyName, params string[] sources)
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
            assemblyName,
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
