#nullable enable

namespace Charon.Console;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentHandlingAttribute : Attribute
{
    public ArgumentHandlingAttribute()
    {
    }

    public ArgumentHandlingAttribute(string? description)
    {
        Description = description;
    }

    public string? Description { get; }

    public bool CaseSensitive { get; set; }

    public bool AllowUnknownArguments { get; set; }
}
