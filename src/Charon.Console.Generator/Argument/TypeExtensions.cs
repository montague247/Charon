using System.Reflection;

namespace Charon;

internal static class TypeExtensions
{
    public static IEnumerable<Type> FindDerivedTypes(this Type baseType)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly =>
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray()!;
                }
                return types;
            })
            .Where(type => type != null
                           && type.IsClass
                           && !type.IsAbstract
                           && baseType.IsAssignableFrom(type));
    }
}
