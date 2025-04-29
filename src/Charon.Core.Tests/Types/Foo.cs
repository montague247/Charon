using System.Text.Json.Serialization;
using Charon.Json;

namespace Charon.Core.Tests.Types
{
    public sealed class Foo
    {
        public string? Hint { get; set; }

        [JsonConverter(typeof(ObjectListJsonConverter<Bar>))]
        public global::System.Collections.Generic.List<Bar>? Bars { get; set; }
    }
}
