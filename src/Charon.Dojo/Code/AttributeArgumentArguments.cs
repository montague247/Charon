namespace Charon.Dojo.Code;

public sealed class AttributeArgumentArguments(string? name, object? value)
{
    public string? Name { get; } = name;

    public object? Value { get; set; } = value;
}
