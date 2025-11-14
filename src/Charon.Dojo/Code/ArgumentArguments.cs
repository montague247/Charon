namespace Charon.Dojo.Code;

public sealed class ArgumentArguments(string name)
{
    public string Name { get; } = name;

    public TypeBlock? Type { get; set; }
}
