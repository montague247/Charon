using System.Text.Json;
using System.Text.Json.Serialization;

namespace Charon.Json
{
    public static class Extensions
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize<T>(value, Options);
        }
    }
}
