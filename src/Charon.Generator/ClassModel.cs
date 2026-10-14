namespace Charon.Generator;

public sealed class ClassModel: IEquatable<ClassModel>
    {
        public string Name { get; }
        public string? Namespace { get; }
        public string Accessibility { get; }
        public string TypeParameters { get; }

        public ClassModel(string name, string? @namespace, string accessibility, string typeParameters)
        {
            Name = name;
            Namespace = @namespace;
            Accessibility = accessibility;
            TypeParameters = typeParameters;
        }

        public bool Equals(ClassModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name &&
                   Namespace == other.Namespace &&
                   Accessibility == other.Accessibility &&
                   TypeParameters == other.TypeParameters;
        }

        public override bool Equals(object? obj) => Equals(obj as ClassModel);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Name.GetHashCode();
                hash = (hash * 397) ^ (Namespace?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ Accessibility.GetHashCode();
                hash = (hash * 397) ^ TypeParameters.GetHashCode();
                return hash;
            }
        }
    }
    