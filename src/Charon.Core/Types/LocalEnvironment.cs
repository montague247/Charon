using System.Text.Json.Serialization;

namespace Charon.Types
{
    public sealed class LocalEnvironment
    {
        private static readonly LocalEnvironment _instance = JsonExtensions.FromFile<LocalEnvironment>(GetPath(), true)!;

        public static LocalEnvironment Instance { get { return _instance; } }

        public string Stage { get; set; } = "DEV";

        public string Area { get; set; } = "Development";

        public string Zone { get; set; } = "Local";

        [JsonIgnore]
        public string FullName { get { return string.Concat(Stage, '-', Area, '-', Zone); } }

        private static string GetPath()
        {
            var path = Path.GetFullPath(".");

#if DEBUG
            if (path.EndsWith(".0", StringComparison.Ordinal))
                path = Path.GetFullPath(Path.Combine(path, "..", "..", ".."));
#endif

            path = Path.GetFullPath(Path.Combine(path, "..", $"localenv-{Path.GetFileName(path)}.json"));

            return path;
        }
    }
}
