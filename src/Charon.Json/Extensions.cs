using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Charon.Json
{
    public static class Extensions
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false);
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize<T>(value, Options);
        }

        public static void ToJson<T>(this T value, string path, bool compact = false)
        {
            if (value == null)
                return;

            using var sw = new StreamWriter(path, false, DefaultEncoding)
            {
                NewLine = "\n"
            };
            using var writer = new Utf8JsonWriter(sw.BaseStream, new() { Indented = !compact });

            JsonSerializer.Serialize(writer, value);

            if (!compact)
                sw.WriteLine();
        }
    }
}
