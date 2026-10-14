using Charon.Generator;
using Microsoft.CodeAnalysis.CSharp;

namespace Charon.Console;

[Generator(LanguageNames.CSharp)]
public sealed class ConsoleArgumentSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{nameof(Templates.ArgumentHandlingAttribute)}.g.cs", Templates.ArgumentHandlingAttribute);
        });

        var candidateClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: static (ctx, ct) => GetClassModel(ctx, ct))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(candidateClasses.Collect(), static (spc, classes) =>
        {
            var seen = new HashSet<string>();
            foreach (var classModel in classes)
            {
                if (classModel is null)
                    continue;

                var key = string.IsNullOrEmpty(classModel.Namespace)
                    ? classModel.Name
                    : $"{classModel.Namespace}.{classModel.Name}";

                if (!seen.Add(key))
                    continue;

                GenerateSource(spc, classModel);
            }
        });
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration
            && classDeclaration.AttributeLists.Count > 0
            && classDeclaration.Modifiers.Any(static m => m.IsKind(SyntaxKind.StaticKeyword))
            && classDeclaration.Modifiers.Any(static m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    private static ClassModel? GetClassModel(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) as INamedTypeSymbol;
        if (symbol is null)
            return null;

        if (symbol.TypeKind != TypeKind.Class || !symbol.IsStatic)
            return null;

        if (!HasArgumentHandlingAttribute(symbol))
            return null;

        var accessibility = symbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public ",
            Accessibility.Internal => "internal ",
            _ => ""
        };

        var typeParameters = symbol.TypeParameters.Length > 0
            ? $"<{string.Join(", ", symbol.TypeParameters.Select(p => p.Name))}>"
            : string.Empty;

        var ns = symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString();

        return new ClassModel(
            symbol.Name,
            ns,
            accessibility,
            typeParameters
        );
    }

    private static bool HasArgumentHandlingAttribute(INamedTypeSymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass is null)
                continue;

            if (attrClass.Name == "ArgumentHandlingAttribute" ||
                attrClass.Name == "ArgumentHandling" ||
                attrClass.ToDisplayString() == "Charon.Console.ArgumentHandlingAttribute")
            {
                return true;
            }
        }

        return false;
    }

    private static void GenerateSource(SourceProductionContext context, ClassModel classModel)
    {
        var hintName = string.IsNullOrEmpty(classModel.Namespace)
            ? $"{classModel.Name}.Sample.g.cs"
            : $"{classModel.Namespace}.{classModel.Name}.Sample.g.cs";

        string source = $$"""
{{classModel.Accessibility}}static partial class {{classModel.Name}}{{classModel.TypeParameters}}
{
    public static void Sample()
    {
        global::System.Console.WriteLine("Sample");
    }
}
""";

        context.AddSource(classModel, source, ".Sample.g.cs");
    }
}
