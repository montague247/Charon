using Serilog;

namespace Charon.Elastic.Bootstrap;

/// <summary>
/// Represents a bootstrap process for creating an Elasticsearch enrollment token.
/// </summary>
public sealed class ElasticsearchEnrollmentTokenBootstrap(string basePath)
    : BootstrapBase(basePath, "escet")
{
    public string? EnrollmentToken { get; private set; }

    protected override string Command => "elasticsearch-create-enrollment-token";

    /// <summary>
    /// Creates an enrollment token for the specified scope.
    /// </summary>
    /// <param name="scope">The scope of this enrollment token, can be either "node" or "kibana"</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public void Execute(string scope, CancellationToken cancellationToken)
    {
        Log.Information("Create enrollment token for '{Scope}'", scope);

        Execute(cancellationToken, "-s", scope).GetAwaiter().GetResult();
    }

    protected override void ProcessOutput(string output, CancellationToken cancellationToken)
    {
        EnrollmentToken = output;
    }
}
