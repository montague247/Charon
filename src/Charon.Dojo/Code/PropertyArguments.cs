namespace Charon.Dojo.Code
{
    public sealed class PropertyArguments(string name)
    {
        public string Name { get; } = name;

        public Accessibility Accessibility { get; set; } = Accessibility.Public;

        public TypeBlock? ReturnType { get; set; }

        public bool Getter { get; set; } = true;

        public bool Setter { get; set; } = true;

        public string? Initial { get; set; }
    }
}
