using Charon.IO;
using Serilog;

namespace Charon.Dojo.Security
{
    public sealed class SecureEncryptConfiguration
    {
        public Dictionary<string, SecureEntryptStageConfiguration>? Stages { get; set; }

        public static SecureEncryptConfiguration Load(string path)
        {
            Log.Information("Load secure encrypt configuration from '{Path}'...", path);

            return JsonExtensions.FromFile<SecureEncryptConfiguration>(path, true)!;
        }

        public void SetStage(string stage, byte[] publicKey)
        {
            Stages ??= [];

            if (!Stages.TryGetValue(stage, out SecureEntryptStageConfiguration? configuration))
            {
                configuration = new();
                Stages.Add(stage, configuration);
            }

            configuration.PublicKey = Convert.ToBase64String(publicKey, 0, publicKey.Length);
        }

        public void Save(string path, CancellationToken cancellationToken)
        {
            var tempPath = string.Concat(path, ".temp");

            this.ToJson(tempPath);

            FileComparer.Move(tempPath, path, cancellationToken);
        }
    }
}
