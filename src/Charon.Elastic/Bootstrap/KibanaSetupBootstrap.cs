using Serilog;

namespace Charon.Elastic.Bootstrap;

public sealed class KibanaSetupBootstrap(string basePath)
    : BootstrapBase(basePath, "kbns")
{
    protected override string Command => "kibana-setup";

    public void Execute(string enrollmentToken, CancellationToken cancellationToken)
    {
        Log.Information("Setup kibana");

        Execute(cancellationToken, "--enrollment-token", enrollmentToken).GetAwaiter().GetResult();
    }
}
