using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Charon
{
    public static class JsonExtensions
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false);
        private static readonly JsonSerializerOptions PrettyOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            DictionaryKeyPolicy = null,
            WriteIndented = true
        };
        private static readonly JsonSerializerOptions CompactOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            DictionaryKeyPolicy = null,
            WriteIndented = false
        };
        private static readonly JsonWriterOptions PrettyWriterOptions = new() { Indented = true };
        private static readonly JsonWriterOptions CompactWriterOptions = new() { Indented = false };

        public static string ToJson<T>(this T value, bool compact = false)
        {
            return JsonSerializer.Serialize<T>(value, compact ? CompactOptions : PrettyOptions);
        }

        public static void ToJson<T>(this T value, string path, bool compact = false)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
                return;

            using var sw = new StreamWriter(path, false, DefaultEncoding)
            {
                NewLine = "\n"
            };
            using var writer = new Utf8JsonWriter(sw.BaseStream, compact ? CompactWriterOptions : PrettyWriterOptions);

            JsonSerializer.Serialize(writer, value, compact ? CompactOptions : PrettyOptions);

            if (!compact)
                sw.WriteLine();
        }

        public static T? FromFile<T>(string path, bool create = true)
            where T : class
        {
            T impl;

            if (File.Exists(path))
                impl = JsonSerializer.Deserialize<T>(File.ReadAllText(path), PrettyOptions)!;
            else
                return create ? Activator.CreateInstance<T>() : default;

            return create && EqualityComparer<T>.Default.Equals(impl, default) ? Activator.CreateInstance<T>() : impl;
        }
    }
}
