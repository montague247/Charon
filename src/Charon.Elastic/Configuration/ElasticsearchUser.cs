using System.Text.Json.Serialization;

namespace Charon.Elastic.Configuration;

public sealed class ElasticsearchUser
{
    [JsonIgnore]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool System { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedUtc { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? PasswordChangedUtc { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Roles { get; set; }

    public bool PasswordChangeRequired()
    {
        return !PasswordChangedUtc.HasValue ||
            string.IsNullOrEmpty(Password) ||
            PasswordChangedUtc.Value.AddDays(30) < DateTime.UtcNow;
    }
}
