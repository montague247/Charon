namespace Charon.Elastic.Configuration;

public sealed class ElasticConfig
{
    public const string DefaultUrl = "http://localhost:9200"; // NOSONAR

    public string Url { get; set; } = DefaultUrl;
}
