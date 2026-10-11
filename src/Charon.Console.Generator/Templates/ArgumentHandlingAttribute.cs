namespace Charon.Console;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentHandlingAttribute : Attribute
{
    public bool CaseSensitive { get; set; }

    public bool AllowUnknownArguments { get; set; }
}
