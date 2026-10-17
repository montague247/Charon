using System.Reflection;

namespace Charon.Console;

internal static class Templates
{
    private static readonly Assembly Assembly = typeof(Templates).Assembly;

    public static string ArgumentProcessorAttribute { get; } = Read("Templates.ArgumentProcessorAttribute.cs");

    public static string ArgumentHandlerAttribute { get; } = Read("Templates.ArgumentHandlerAttribute.cs");

    public static string IArgumentHandler { get; } = Read("Templates.IArgumentHandler.cs");

    public static string IGlobalOptions { get; } = Read("Templates.IGlobalOptions.cs");

    private static string Read(string name)
    {
        using var stream = Assembly.GetManifestResourceStream(name)?? throw new InvalidOperationException($"Embedded resource '{name}' not found.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}