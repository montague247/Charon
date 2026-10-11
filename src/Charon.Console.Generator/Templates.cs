using System.Reflection;

namespace Charon.Console;

internal static class Templates
{
    private static readonly Assembly Assembly = typeof(Templates).Assembly;

    public static string ArgumentHandlingAttribute { get; } = Read("Templates.ArgumentHandlingAttribute.cs");

    private static string Read(string name)
    {
        using var stream = Assembly.GetManifestResourceStream(name)?? throw new InvalidOperationException($"Embedded resource '{name}' not found.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}