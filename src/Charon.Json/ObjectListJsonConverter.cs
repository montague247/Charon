using System.Text.Json;
using System.Text.Json.Serialization;

namespace Charon.Json
{
    public sealed class ObjectListJsonConverter<T> : JsonConverter<List<T>>
    {
        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null ||
                reader.TokenType == JsonTokenType.StartArray)
                return JsonSerializer.Deserialize<List<T>>(ref reader, options);
            else
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);

                if (item == null)
                    return default;

                return new() { item };
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            if (value == null ||
                value.Count > 1)
                JsonSerializer.Serialize(writer, value, options);
            else
                JsonSerializer.Serialize(writer, value[0], options);
        }
    }
}
