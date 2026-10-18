using Charon.Generator;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Charon.Console;

[Generator(LanguageNames.CSharp)]
public sealed class ConsoleArgumentSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{nameof(Templates.ArgumentProcessorAttribute)}.g.cs", Templates.ArgumentProcessorAttribute);
        });

        var classModelProvider = context.CompilationProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(static (tuple, _) => GetClassModel(tuple.Left, tuple.Right));

        context.RegisterSourceOutput(classModelProvider, static (spc, classModel) =>
        {
            if (classModel is null)
                return;

            GenerateSource(spc, classModel);

            spc.AddSource($"{nameof(Templates.ArgumentHandlerAttribute)}.g.cs", Templates.ArgumentHandlerAttribute);
            spc.AddSource($"{nameof(Templates.IArgumentHandler)}.g.cs", Templates.IArgumentHandler);
            spc.AddSource($"{nameof(Templates.IGlobalOptions)}.g.cs", Templates.IGlobalOptions);
        });
    }

    private static ClassModel? GetClassModel(Compilation compilation, AnalyzerConfigOptionsProvider options)
    {
        var attribute = GetArgumentProcessorAttribute(compilation.Assembly);
        if (attribute is null)
            return null;

        var name = "ArgumentProcessor";

        if (attribute.ConstructorArguments.Length >= 2 &&
            attribute.ConstructorArguments[1].Value is string customName &&
            !string.IsNullOrWhiteSpace(customName))
        {
            name = customName;
        }

        string? ns = null;
        if (options.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace) &&
            !string.IsNullOrWhiteSpace(rootNamespace))
        {
            ns = rootNamespace;
        }
        else if (!string.IsNullOrWhiteSpace(compilation.AssemblyName))
        {
            ns = compilation.AssemblyName;
        }

        return new ClassModel(
            name,
            ns,
            "public ",
            string.Empty
        );
    }

    private static AttributeData? GetArgumentProcessorAttribute(IAssemblySymbol assembly)
    {
        foreach (var attribute in assembly.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;

            if (attrClass is null)
                continue;

            if (attrClass.Name == "ArgumentProcessorAttribute" ||
                attrClass.Name == "ArgumentProcessor" ||
                attrClass.ToDisplayString() == "Charon.Console.ArgumentProcessorAttribute")
            {
                return attribute;
            }
        }

        return null;
    }

    private static void GenerateSource(SourceProductionContext context, ClassModel classModel)
    {
        string source = $$""""
{{classModel.Accessibility}}static partial class {{classModel.Name}}{{classModel.TypeParameters}}
{
    public static void Sample()
    {
        global::System.Console.WriteLine("""
{{Banner.Create("Sample")}}
""");
    }
}
"""";

        context.AddSource(classModel, source, ".Sample.g.cs");
    }
}
