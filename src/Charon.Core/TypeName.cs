using System.Text;

namespace Charon
{
    public sealed class TypeName
    {
        private static TypeName? _instance;
        private static readonly Dictionary<Type, string> _simpleTypes = new()
        {
            { typeof(string), "string" },
            { typeof(int), "int" },
            { typeof(int), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(decimal), "decimal" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(short), "ushort" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(char), "char" }
        };
        private readonly HashSet<string> _namespaces = [];
        private readonly Dictionary<Type, string> _cachedNames = [];

        public TypeName(params string[] namespaces)
        {
            foreach (var name in namespaces)
            {
                AddNamespace(name);
            }
        }

        public static TypeName Instance
        {
            get
            {
                _instance ??= new TypeName();

                return _instance;
            }
        }

        public IEnumerable<string> Namespaces
        {
            get
            {
                return _namespaces.OrderBy(s => s);
            }
        }

        public static bool IsSimple<T>()
        {
            return IsSimple(typeof(T));
        }

        public static bool IsSimple(Type type)
        {
            if (_simpleTypes.ContainsKey(type))
                return true;

            if (type.IsArray)
                return IsSimple(type.GetElementType()!);

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimple(type.GetGenericArguments()[0]);

            return false;
        }

        public bool AddNamespace(string name)
        {
            return _namespaces.Add(name);
        }

        public string? GetName(Enum? e)
        {
            if (e == null)
                return default;

            return string.Concat(GetName(e.GetType()), '.', e.ToString());
        }

        public string GetName<T>()
        {
            return GetName(typeof(T))!;
        }

        public string? GetName(Type? type)
        {
            if (type == null)
                return default;

            if (_cachedNames.TryGetValue(type, out var typeName))
                return typeName;

            typeName = GetSimpleOrArrayTypeName(type) ?? GetGenericTypeName(type) ?? type.Name;

            var typeNamespace = type.Namespace!;
            typeName = ResolveNamespaceConflict(type, typeName, typeNamespace);

            _cachedNames.TryAdd(type, typeName);

            return typeName;
        }

        private string? GetSimpleOrArrayTypeName(Type type)
        {
            if (_simpleTypes.TryGetValue(type, out var typeName))
                return typeName;

            if (type.IsArray)
                return string.Concat(GetName(type.GetElementType()!), "[]");

            return null;
        }

        private string? GetGenericTypeName(Type type)
        {
            if (!type.IsGenericType)
                return null;

            var genericArguments = type.GetGenericArguments();

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return string.Concat(GetName(genericArguments[0]), '?');

            var sb = new StringBuilder(type.Name[..type.Name.IndexOf('`')]).Append('<');
            var first = true;

            foreach (var argumentType in genericArguments)
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");

                sb.Append(GetName(argumentType));
            }

            return sb.Append('>').ToString();
        }

        private string ResolveNamespaceConflict(Type type, string typeName, string typeNamespace)
        {
            if (_namespaces.Contains(typeNamespace))
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(s => !s.IsDynamic)
                    .SelectMany(s => s.GetTypes())
                    .Where(s => s.IsPublic && s.Namespace != null && _namespaces.Contains(s.Namespace))
                    .OrderBy(s => s.FullName)
                    .ToArray();

                var matchingTypes = types.Where(s => string.Compare(s.Name, type.Name, StringComparison.Ordinal) == 0).ToArray();

                if (matchingTypes.Length > 1)
                    return string.Concat(type.Namespace, '.', typeName);
            }

            return string.Concat(typeNamespace, '.', typeName);
        }
    }
}
