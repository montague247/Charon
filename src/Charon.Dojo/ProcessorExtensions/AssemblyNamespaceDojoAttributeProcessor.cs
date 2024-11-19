using System.Reflection;

namespace Charon.Dojo.ProcessorExtensions
{
    public static class AssemblyNamespaceDojoAttributeProcessor
    {
        public static void Process(this AssemblyNamespaceDojoAttribute attribute, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(s => string.Compare(s.Namespace, attribute.Namespace, StringComparison.Ordinal) == 0))
            {
                
            }
        }
    }
}
