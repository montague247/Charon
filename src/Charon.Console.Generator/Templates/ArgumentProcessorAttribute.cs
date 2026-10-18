#nullable enable

using System;

namespace Charon.Console;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentProcessorAttribute : Attribute
{
    public ArgumentProcessorAttribute()
    {
    }

    public ArgumentProcessorAttribute(string? application, string name = "ArgumentProcessor")
    {
        Application = application;
        Name = name;
    }

    public string? Application { get; }

    public string Name { get; } = "ArgumentProcessor";

    public bool CaseSensitive { get; set; }

    public bool AllowUnknownArguments { get; set; }
}
