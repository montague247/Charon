namespace Charon.Dojo.Code
{
    public sealed class CodeFileArguments
    {
        public HashSet<string>? Usings { get; set; }

        public string? NamespaceName { get; set; }

        public TypeName TypeName { get; } = new();

        public void AddUsing(string name)
        {
            Usings ??= [];
            Usings.Add(name);

            TypeName.AddNamespace(name);
        }
    }
}
