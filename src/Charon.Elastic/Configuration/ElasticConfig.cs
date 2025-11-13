using System.Text.Json.Serialization;
using Charon.Types;

namespace Charon.Elastic.Configuration;

public sealed class ElasticConfig : ConnectionsSettings
{
    public const string DefaultUrl = "http://localhost:9200"; // NOSONAR

    public string Url { get; set; } = DefaultUrl;

    [JsonRequired]
    public string? Username { get; set; }

    [Secure]
    [JsonRequired]
    public string? Password { get; set; }

    [JsonIgnore]
    public bool HasPassword { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConnectionsSettings LoggingSettings { get; set; } = new ConnectionsSettings();

    public bool LogLoggingApiCalls { get; set; }
}
