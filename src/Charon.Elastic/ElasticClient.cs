using Charon.Types;

namespace Charon.Elastic
{
    public sealed class ElasticClient
    {
        private static readonly object _lock = new();
        private static ElasticClient? _client;

        public string ConfigPath { get; set; } = Path.GetFullPath(Path.Combine("..", $"elasticclient-{LocalEnvironment.Instance.FullName}.json"));

        public static ElasticClient Instance
        {
            get
            {
                if (_client == null)
                {
                    lock (_lock)
                    {
                        if (_client != null)
                            return _client;
                        _client = new();
                    }
                }

                return _client;
            }
        }
    }
}
