#nullable enable

using System;

namespace Charon.Console;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentProcessorAttribute : Attribute
{
    public ArgumentProcessorAttribute()
    {
    }

    public ArgumentProcessorAttribute(string? description)
    {
        Description = description;
    }

    public string? Description { get; }

    public bool CaseSensitive { get; set; }

    public bool AllowUnknownArguments { get; set; }
}
