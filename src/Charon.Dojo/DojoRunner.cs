using System.Reflection;
using Charon.Dojo.ProcessorExtensions;
using Serilog;

namespace Charon.Dojo
{
    public static class DojoRunner
    {
        public static void Execute()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
#if DEBUG
                .WriteTo.Debug()
#endif
                .CreateLogger();

            Log.Information("Do all the dojo stuff...");

            var startedUtc = DateTime.UtcNow;
            var assemblyNames = AppDomain.CurrentDomain.GetAssemblies().Where(s => !s.IsDynamic && !ExcludeAssembly(s.GetName().Name)).Select(s => s.GetName().Name).ToHashSet();
            var baseAssembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(baseAssembly.Location)!;

            foreach (var fileName in Directory.GetFiles(path, "*.dll"))
            {
                var name = AssemblyName.GetAssemblyName(fileName);

                if (ExcludeAssembly(name.Name) ||
                    !assemblyNames.Add(name.Name))
                    continue;

                Log.Information("Load assembly '{Name}'...", name.Name);

                AppDomain.CurrentDomain.Load(name);
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(s => assemblyNames.Contains(s.GetName().Name)).OrderBy(s => s.GetName().Name))
            {
                Log.Information("Process assembly {Name}...", assembly.GetName().Name);

                ProcessAssemblyAttributes(assembly);
            }

            Log.Information("Did all the dojo stuff after {Duration}!", DateTime.UtcNow - startedUtc);
        }

        private static void ProcessAssemblyAttributes(Assembly assembly)
        {
            ProcessAssemblyAttributes(assembly, assembly.GetCustomAttributes<AssemblySecurityDojoAttribute>());
            ProcessAssemblyAttributes(assembly, assembly.GetCustomAttributes<AssemblyNamespaceDojoAttribute>());
        }

        private static void ProcessAssemblyAttributes(Assembly assembly, IEnumerable<AssemblySecurityDojoAttribute> attributes)
        {
            if (!attributes.Any())
                return;

            var count = attributes.Count();
            var name = attributes.First().GetType().Name;

            Log.Information($"Process {{Count}} {{Name}}{Plural(count)}...", name, count);

            var current = 0;

            foreach (var attribute in attributes)
            {
                attribute.EnsureNamespace(assembly);

                Log.Information("[{Current}/{Count}] Process namespace '{Name}'...", ++current, count, attribute.Namespace);

                attribute.Process();
            }
        }

        private static void ProcessAssemblyAttributes(Assembly assembly, IEnumerable<AssemblyNamespaceDojoAttribute> attributes)
        {
            if (!attributes.Any())
                return;

            var count = attributes.Count();
            var name = attributes.First().GetType().Name;

            Log.Information($"Process {{Count}} {{Name}}{Plural(count)}...", name, count);

            var current = 0;

            foreach (var attribute in attributes)
            {
                Log.Information("[{Current}/{Count}] Process namespace '{Name}'...", ++current, count, attribute.Namespace);

                attribute.Process(assembly);
            }
        }

        private static string? Plural(int count)
        {
            return count == 1 ? null : "s";
        }

        private static bool ExcludeAssembly(string? name)
        {
            return name == null ||
                    string.Compare(name, "netstandard", StringComparison.Ordinal) == 0 ||
                    string.Compare(name, "testhost", StringComparison.Ordinal) == 0 ||
                    name.StartsWith("Microsoft", StringComparison.Ordinal) ||
                    name.StartsWith("Newtonsoft", StringComparison.Ordinal) ||
                    name.StartsWith("Serilog", StringComparison.Ordinal) ||
                    name.StartsWith("System", StringComparison.Ordinal) ||
                    name.StartsWith("xunit", StringComparison.Ordinal);
        }
    }
}
