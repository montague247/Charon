using Charon.Generator;

namespace Charon.Console;

public sealed class ArgumentProcessorModel(
    string name,
    string? @namespace,
    string accessibility,
    string typeParameters,
    string application)
    : ClassModel(name, @namespace, accessibility, typeParameters), IEquatable<ArgumentProcessorModel>
{
    public string Application { get; } = application;

    public bool Equals(ArgumentProcessorModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Application == other.Application;
    }

    public override bool Equals(object? obj) => Equals(obj as ArgumentProcessorModel);

    public override int GetHashCode()
    {
        unchecked
        {
            return (base.GetHashCode() * 397) ^ Application.GetHashCode();
        }
    }
}
