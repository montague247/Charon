using Charon.Elastic.Configuration;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Security;
using Elastic.Transport;

namespace Charon.Elastic.Bootstrap;

public sealed class ElasticsearchUserPassword
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchUserPassword(params ElasticsearchUser[] users)
    {
        var user = users.Single(s => string.Compare(s.Username, "elastic", StringComparison.Ordinal) == 0);

        _client = new(new ElasticsearchClientSettings(new Uri(ElasticConfig.DefaultUrl))
                .Authentication(new BasicAuthentication(user.Username!, user.Password.Decrypt()!))
                .DisableAutomaticProxyDetection()
                .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
                .EnableHttpCompression());
    }

    public void SetRandomPassword(ElasticsearchUser user, CancellationToken cancellationToken)
    {
        var newPassword = SecurityExtensions.CreateSecurePassword(32);

        _client.Security.ChangePasswordAsync(new ChangePasswordRequest(user.Username!)
        {
            Password = newPassword
        }, cancellationToken).GetAwaiter().GetResult();

        user.Password = newPassword;
        user.PasswordChangedUtc = DateTime.UtcNow;
    }
}
