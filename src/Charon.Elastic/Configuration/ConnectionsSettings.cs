using System.Text.Json.Serialization;

namespace Charon.Elastic.Configuration;

public class ConnectionsSettings
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConnectionLimit { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int RequestTimeout { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int MaxRetryTimeout { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int MaximumRetries { get; set; }
}
