namespace Charon.Dojo
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class AssemblyNamespaceDojoAttribute(string namespaceName) : Attribute
    {
        public string Namespace { get; } = namespaceName;

        public bool JsonTests { get; set; } = true;
    }
}
