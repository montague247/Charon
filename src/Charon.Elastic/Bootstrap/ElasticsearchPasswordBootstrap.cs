using Charon.Elastic.Configuration;
using Serilog;

namespace Charon.Elastic.Bootstrap;

public sealed class ElasticsearchPasswordBootstrap(string basePath)
    : BootstrapBase(basePath, "esrp")
{
    private ElasticsearchUser? _user;

    protected override string Command => "elasticsearch-reset-password";

    public void Execute(ElasticsearchUser user, CancellationToken cancellationToken)
    {
        _user = user;

        Log.Information("Reset password for '{Name}'", _user.Username);

        LogOutput = false;

        Execute(cancellationToken, "-a", "-b", "-s", "-u", user.Username!).GetAwaiter().GetResult();
    }

    protected override void ProcessOutput(string output, CancellationToken cancellationToken)
    {
        if (_user == null)
            return;

        _user.Password = output;
        _user.PasswordChangedUtc = DateTime.UtcNow;
    }
}
