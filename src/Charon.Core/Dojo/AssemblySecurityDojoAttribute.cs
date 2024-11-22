using System.Reflection;
using System.Runtime.CompilerServices;

namespace Charon.Dojo
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class AssemblySecurityDojoAttribute(string name, string? path = null, string? namespaceName = null, [CallerFilePath] string? sourceFilePath = null) : Attribute
    {
        public string Name { get; } = name;

        public string? Path { get; } = path;

        public string? Namespace { get; private set; } = namespaceName;

        public string SourcePath { get; } = System.IO.Path.GetDirectoryName(sourceFilePath)!;

        public string[] Stages { get; set; } = ["DEV"];

        public string? ConfigurationPath { get; set; }

        public int KeySize { get; set; } = 4096;

        public int Priority { get; set; } = int.MaxValue;

        public void EnsureNamespace(Assembly assembly)
        {
            if (Namespace != null)
                return;

            var name = assembly.GetName().Name!;

            if (string.IsNullOrEmpty(Path))
                Namespace = name;
            else
                Namespace = string.Concat(name, '.', Path.Replace(System.IO.Path.DirectorySeparatorChar, '.'));
        }
    }
}
