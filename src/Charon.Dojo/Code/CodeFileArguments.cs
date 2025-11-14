namespace Charon.Dojo.Code;

public sealed class CodeFileArguments
{
    public HashSet<string>? Usings { get; set; }

    public string? NamespaceName { get; set; }

    public TypeName TypeName { get; } = new();

    public void AddUsing(string name)
    {
        TypeName.AddNamespace(name);

        if (string.Compare(name, "System", StringComparison.Ordinal) == 0)
            return;

        Usings ??= [];
        Usings.Add(name);
    }
}
