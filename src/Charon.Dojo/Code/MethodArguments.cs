namespace Charon.Dojo.Code
{
    public sealed class MethodArguments(string name)
    {
        public string Name { get; } = name;

        public Accessibility Accessibility { get; set; } = Accessibility.Private;

        public TypeBlock? ReturnType { get; set; }
    }
}
