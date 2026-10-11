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
    }
}
