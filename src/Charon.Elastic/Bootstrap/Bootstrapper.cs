using Charon.Elastic.Configuration;
using Serilog;

namespace Charon.Elastic.Bootstrap;

public static class Bootstrapper
{
    public static async Task Execute(string[] args, CancellationToken cancellationToken)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
#if DEBUG
            .WriteTo.Debug()
#endif
            .CreateLogger();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "es":
                case "elastic":
                case "elasticsearch":
                    var version = GetArgument("version", args, ref i);

                    await StartElasticsearch(version, cancellationToken);
                    break;
            }
        }
    }

    private static async Task StartElasticsearch(string? version, CancellationToken cancellationToken)
    {
        var elasticsearchPath = FindPath("elasticsearch", version);

        if (Exit(elasticsearchPath == null || !Directory.Exists(elasticsearchPath), () => Log.Error("No path found: {Path}", elasticsearchPath), 1))
            return;

        Log.Information("Start elasticsearch in path '{Path}'", elasticsearchPath);

        var starter = new ElasticsearchBootstrap(elasticsearchPath!)
        {
            GetKibanaPath = () => FindPath("kibana", version),
            GetUsers = () => [.. ElasticsearchBootstrapConfig.Load(elasticsearchPath!).Users!.Values.Where(s => s.PasswordChangeRequired())],
            SaveUsers = users => ElasticsearchBootstrapConfig.SetUsers(elasticsearchPath!, cancellationToken, users),
            KibanaEnrollmentRequired = () => !ElasticsearchBootstrapConfig.Load(elasticsearchPath!).KibanaEnrollmentUtc.HasValue,
            KibanaEnrollmentFinished = () => ElasticsearchBootstrapConfig.SetKibanaEnrollmentFinished(elasticsearchPath!, DateTime.UtcNow, cancellationToken)
        };

        await starter.Execute(cancellationToken);
    }

    private static bool Exit(bool exit, Action handleExit, int exitCode)
    {
        if (!exit)
            return false;

        handleExit();

        Environment.Exit(exitCode);

        return true;
    }

    private static string? FindPath(string name, string? version)
    {
        var path = Environment.CurrentDirectory;

        Log.Information("Find directory '{Name}' with version '{Version}' in path '{Path}'", name, version, path);

        return Directory.GetDirectories(path, $"{name}-{version}*").OrderByDescending(s => s).FirstOrDefault();
    }

    private static string? GetArgument(string name, string[] args, ref int i)
    {
        if (i + 1 == args.Length)
            return default;

        var value = args[i + 1];

        if (!value.StartsWith("--") ||
            string.Compare(value[2..], name, StringComparison.Ordinal) != 0)
            return default;

        i++;

        return args[++i];
    }
}
