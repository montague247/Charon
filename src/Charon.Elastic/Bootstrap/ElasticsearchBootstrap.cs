using Charon.Elastic.Configuration;
using Serilog;

namespace Charon.Elastic.Bootstrap;

public sealed class ElasticsearchBootstrap(string basePath)
    : BootstrapBase(basePath, "es")
{
    private readonly string _basePath = basePath;
    private bool _started;
    private bool _generatePasswords;
    private bool _kibanaEnrollment;

    public Func<string?>? GetKibanaPath { get; set; }

    public Func<ElasticsearchUser[]>? GetUsers { get; set; }

    public Action<ElasticsearchUser[]>? SaveUsers { get; set; }

    public Func<bool>? KibanaEnrollmentRequired { get; set; }

    public Action? KibanaEnrollmentFinished { get; set; }

    protected override string Command => "elasticsearch";

    public override async Task Execute(CancellationToken cancellationToken, params string[] args)
    {
        _generatePasswords = GetUsers != null &&
            SaveUsers != null;
        _kibanaEnrollment = GetKibanaPath != null &&
            KibanaEnrollmentRequired != null &&
            KibanaEnrollmentFinished != null &&
            KibanaEnrollmentRequired();

        await base.Execute(cancellationToken, args);
    }

    protected override void ProcessOutput(string output, CancellationToken cancellationToken)
    {
        if (!_started &&
            output.Contains("current.health=\"GREEN\"", StringComparison.Ordinal) &&
            output.Contains("shards started", StringComparison.Ordinal))
        {
            _started = true;

            Log.Information(@"
  ______ _           _   _                              _           _             _           _ 
 |  ____| |         | | (_)                            | |         | |           | |         | |
 | |__  | | __ _ ___| |_ _  ___ ___  ___  __ _ _ __ ___| |__    ___| |_ __ _ _ __| |_ ___  __| |
 |  __| | |/ _` / __| __| |/ __/ __|/ _ \/ _` | '__/ __| '_ \  / __| __/ _` | '__| __/ _ \/ _` |
 | |____| | (_| \__ \ |_| | (__\__ \  __/ (_| | | | (__| | | | \__ \ || (_| | |  | ||  __/ (_| |
 |______|_|\__,_|___/\__|_|\___|___/\___|\__,_|_|  \___|_| |_| |___/\__\__,_|_|   \__\___|\__,_|");

            if (_kibanaEnrollment)
                EnrollKibana(cancellationToken);

            if (_generatePasswords)
                GeneratePasswords(cancellationToken);
        }
    }

    private void GeneratePasswords(CancellationToken cancellationToken)
    {
        _generatePasswords = false;

        var users = GetUsers!();

        if (users == null ||
            users.Length == 0)
            return;

        Log.Information("Generate passwords of {Count} users", users.Length);

        var bootstrap = new ElasticsearchPasswordBootstrap(_basePath);
        var elasticUser = new ElasticsearchUserPassword(users);

        Task.Run(() =>
        {
            foreach (var user in users)
            {
                if (user.System)
                    bootstrap.Execute(user, cancellationToken);
                else
                    elasticUser.SetRandomPassword(user, cancellationToken);
            }

            SaveUsers!(users);

            Log.Information("Generated passwords of {Count} users", users.Length);
        }, cancellationToken);
    }

    private void EnrollKibana(CancellationToken cancellationToken)
    {
        _kibanaEnrollment = false;

        var tokenBootstrap = new ElasticsearchEnrollmentTokenBootstrap(_basePath);

        tokenBootstrap.Execute("kibana", cancellationToken);

        if (string.IsNullOrEmpty(tokenBootstrap.EnrollmentToken))
        {
            Log.Error("No enrollment token returned");

            return;
        }

        var kibanaBasePath = GetKibanaPath!();

        if (string.IsNullOrEmpty(kibanaBasePath))
        {
            Log.Warning("No kibana path found!");

            return;
        }

        var kibanaSetupBootstrap = new KibanaSetupBootstrap(kibanaBasePath);

        kibanaSetupBootstrap.Execute(tokenBootstrap.EnrollmentToken, cancellationToken);

        KibanaEnrollmentFinished!();
    }
}
