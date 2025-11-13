using System.Text.Json.Serialization;

namespace Charon.Elastic.Configuration;

public sealed class ElasticsearchUser
{
    [JsonIgnore]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedUtc { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? PasswordChangedUtc { get; set; }
}
