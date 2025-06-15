using System.Reflection;
using Serilog;

namespace Charon;

public static class TypeExtensions
{
    public static string? TypeName(this Type type, bool withoutVersion = true)
    {
        if (type == null)
            return default;

        var fullName = type.FullName!;

        if (withoutVersion)
        {
            int startIdx;

            while ((startIdx = fullName.IndexOf(", Version=", StringComparison.Ordinal)) != -1)
            {
                var endIdx = fullName.IndexOf(", PublicKeyToken=", StringComparison.Ordinal) + 17;
                var pkt = fullName.Substring(endIdx, 4);

                if (string.Compare(pkt, "null", StringComparison.Ordinal) == 0)
                    endIdx += 4;
                else
                    endIdx += 16;

                fullName = string.Concat(fullName[..startIdx], fullName[endIdx..]);
            }
        }

        return string.Concat(fullName, ", ", type.Assembly.GetName().Name);
    }

    public static IEnumerable<Type> FindDerivedTypes(this Type baseType)
    {
        if (!baseType.IsClass && !baseType.IsGenericTypeDefinition && !baseType.IsInterface)
            throw new ArgumentException($"'{baseType.FullName}' must be a class type, an interface type or generic type definition.", nameof(baseType));

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly =>
            {
#if DEBUG
                var name = assembly.GetName().Name ?? "Unknown Assembly";

                if (!name.StartsWith("System.", StringComparison.Ordinal) &&
                    !name.StartsWith("Microsoft.", StringComparison.Ordinal) &&
                    !name.StartsWith("Newtonsoft.", StringComparison.Ordinal) &&
                    !name.StartsWith("netstandard", StringComparison.Ordinal) &&
                    !name.StartsWith("testhost", StringComparison.Ordinal) &&
                    !name.StartsWith("Anonymously", StringComparison.Ordinal) &&
                    !name.StartsWith("xunit.", StringComparison.Ordinal))
                    Log.Information("Loading types from assembly {AssemblyName}", name);
#endif

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
                           && InheritsFrom(type, baseType));
    }

    private static bool InheritsFrom(Type type, Type baseType)
    {
        if (baseType.IsClass)
            return baseType.IsAssignableFrom(type);

        if (baseType.IsInterface || baseType.IsGenericTypeDefinition)
        {
            // get all interfaces implemented by the type
            var interfaces = type.GetInterfaces();

            // check if the type implements the base type as an interface as a generic interface
            if (interfaces.Any(i => i == baseType || (i.IsGenericType && i.GetGenericTypeDefinition() == baseType)))
                return true;
        }

        // check if the type is a generic type and if it matches the base type
        var current = type;

        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == baseType)
                return true;

            current = current.BaseType;
        }

        return false;
    }
}
