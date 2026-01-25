using Charon.IO;
using Serilog;

namespace Charon.Elastic.Configuration;

public sealed class ElasticsearchBootstrapConfig
{
    public Dictionary<string, ElasticsearchUser>? Users { get; set; }

    public DateTime? KibanaEnrollmentUtc { get; set; }

    public static ElasticsearchBootstrapConfig Load(string path)
    {
        var config = JsonExtensions.FromFile<ElasticsearchBootstrapConfig>(GetGetFileName(path), false) ?? new();

        config.Users ??= new Dictionary<string, ElasticsearchUser>
        {
            { "elastic", new() { System = true } },
            { "kibana", new() { System = true } }
        };

        foreach (var user in config.Users)
        {
            user.Value.Username = user.Key;
            user.Value.CreatedUtc ??= DateTime.UtcNow;
        }

        return config;
    }

    public static void SetUsers(string path, CancellationToken cancellationToken, params ElasticsearchUser[] users)
    {
        if (users == null ||
            users.Length == 0)
            return;

        var config = Load(path);

        foreach (var user in users)
        {
            if (!config.Users!.TryGetValue(user.Username!, out ElasticsearchUser? existingUser))
            {
                existingUser = user;
                config.Users.Add(user.Username!, existingUser);
            }

            existingUser.Password = user.Password;
            existingUser.PasswordChangedUtc = user.PasswordChangedUtc;

            var bellerophonPath = Path.GetFullPath(Path.Combine(path, "..", "..", "Projects", "bellerophon"));

            if (Directory.Exists(bellerophonPath))
            {
                SetPassword(Path.Combine(bellerophonPath, "elastic-config-tests.json"), user, cancellationToken);
                SetPassword(Path.Combine(bellerophonPath, "Bellerophon-elastic-config-DEV-VW-INTRA.json"), user, cancellationToken);
            }
        }

        config.Save(path, cancellationToken);
    }

    public static void SetKibanaEnrollmentFinished(string path, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var config = Load(path);

        config.KibanaEnrollmentUtc = nowUtc;

        config.Save(path, cancellationToken);
    }

    public void Save(string path, CancellationToken cancellationToken)
    {
        var fileName = GetGetFileName(path);
        var tempFileName = string.Concat(fileName, ".temp");

        Log.Information("Save config '{Path}'", fileName);

        this.ToJson(tempFileName);

        FileComparer.Move(tempFileName, fileName, cancellationToken);
    }

    private static string GetGetFileName(string path)
    {
        return Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileName(path)!}-bootstrap.json");
    }

    private static void SetPassword(string path, ElasticsearchUser user, CancellationToken cancellationToken)
    {
        var config = JsonExtensions.FromFile<ElasticConfig>(path, false);

        if (config == null ||
            string.Compare(config.Username, user.Username, StringComparison.Ordinal) != 0)
            return;

        Log.Warning("Set password for '{Name}' user in file '{Path}'", user.Username, path);

        config.Password = user.Password;

        var tempPath = string.Concat(path, ".temp");

        config.ToJson(tempPath);

        FileComparer.Move(tempPath, path, cancellationToken);
    }
}
