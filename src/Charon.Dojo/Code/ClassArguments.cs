namespace Charon.Dojo.Code
{
    public sealed class ClassArguments(string name)
    {
        public string Name { get; } = name;

        public Accessibility Accessibility { get; set; } = Accessibility.Private;

        public Inheritance Inheritance { get; set; } = Inheritance.Default;

        public bool Partial { get; set; }

        public List<TypeBlock>? Inherits { get; set; }
    }
}
