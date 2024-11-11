using System.Text.Json.Serialization;

namespace Charon.Json.Tests
{
    public sealed class Foo
    {
        public string? Hint { get; set; }

        [JsonConverter(typeof(ObjectListJsonConverter<Bar>))]
        public List<Bar>? Bars { get; set; }
    }
}
