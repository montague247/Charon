using System.Security.Cryptography;
using Charon.Dojo.Code;
using Charon.Dojo.Security;
using Charon.Security;
using Charon.Types;
using Serilog;

namespace Charon.Dojo.ProcessorExtensions
{
    public static class AssemblySecurityDojoAttributeProcessor
    {
        public static void Process(this AssemblySecurityDojoAttribute attribute)
        {
            var path = Path.GetFullPath(Path.Combine(attribute.SourcePath, "..", attribute.ConfigurationPath ?? Path.Combine("SecureEncrypt.json")));
            var configuration = SecureEncryptConfiguration.Load(path);
            var stagePrivateKeys = new Dictionary<string, byte[]>();

            foreach (var stage in attribute.Stages)
            {
                RSACryptoServiceProvider cryptoServiceProvider;

                var keyPath = Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}-{stage}.xml");

                if (File.Exists(keyPath))
                {
                    Log.Information("Load key from '{Path}'...", keyPath);

                    cryptoServiceProvider = new RSACryptoServiceProvider();
                    cryptoServiceProvider.FromXmlString(File.ReadAllText(keyPath));
                }
                else
                {
                    Log.Information("Generate key and save to '{Path}'...", keyPath);

                    var startedUtc = DateTime.UtcNow;
                    cryptoServiceProvider = new RSACryptoServiceProvider(attribute.KeySize);

                    File.WriteAllText(keyPath, cryptoServiceProvider.ToXmlString(true));

                    Log.Information("Generated key and saved to '{Path}' after {Duration}", keyPath, DateTime.UtcNow - startedUtc);
                }

                stagePrivateKeys.Add(stage, cryptoServiceProvider.ExportCspBlob(true));
                configuration.SetStage(stage, cryptoServiceProvider.ExportCspBlob(false));
            }

            configuration.Save(path, default);

            GenerateFiles(attribute, configuration, stagePrivateKeys);
        }

        private static void GenerateFiles(AssemblySecurityDojoAttribute attribute, SecureEncryptConfiguration configuration, Dictionary<string, byte[]> stagePrivateKeys)
        {
            if (configuration.Stages == null)
                return;

            var defaultStage = configuration.Stages.First().Key;

            var basePath = attribute.Path == null ? attribute.SourcePath : Path.Combine(attribute.SourcePath, attribute.Path);
            Directory.CreateDirectory(basePath);

            var path = Path.Combine(attribute.SourcePath, attribute.Path ?? string.Empty, $"{attribute.Name}PrivateKeyRetriever.cs");
            var privateKey = stagePrivateKeys[defaultStage];
            var privateHash = SecurityExtensions.CreateRandom(privateKey.Length);

            new CodeWriter(s => s.Namespace(attribute.Namespace!))
                .Class($"{attribute.Name}PrivateKeyRetriever", s => s
                    .Accessibility(Accessibility.Public)
                    .Inheritance(Inheritance.Sealed)
                    .Inherit<IPrivateKeyRetriever>())
                    .Attribute<PriorityAttribute>(s => s
                        .Argument(attribute.Priority)
                    )
                    .Property<string>("DefaultStage", s => s.Setter(false).Initial(defaultStage.ToEscapedString()))
                    .Method<byte[]>("GetKey", s => s.Accessibility(Accessibility.Public), code =>
                    {
                        code.WriteLine($"return [{string.Join(", ", privateKey.Veil(privateHash))}];");
                    })
                    .Method<byte[]>("GetHash", s => s.Accessibility(Accessibility.Public), code =>
                    {
                        code.WriteLine($"return [{string.Join(", ", privateHash)}];");
                    })
                .ToFile(path, default);

            path = Path.Combine(attribute.SourcePath, attribute.Path ?? string.Empty, $"{attribute.Name}PublicKeyRetriever.cs");

            new CodeWriter(s => s.Namespace(attribute.Namespace!))
                .Class($"{attribute.Name}PublicKeyRetriever", s => s
                    .Accessibility(Accessibility.Public)
                    .Inheritance(Inheritance.Sealed)
                    .Inherit<IPublicKeyRetriever>())
                    .Attribute<PriorityAttribute>(s => s
                        .Argument(attribute.Priority)
                    )
                    .Method<byte[]>("GetKey", s => s
                        .Accessibility(Accessibility.Public)
                        .Argument<string>("stage"), code =>
                    {
                        code.StartSwitch("return", "stage");

                        foreach (var stage in configuration.Stages)
                        {
                            var publicKey = Convert.FromBase64String(stage.Value.PublicKey!);

                            code.WriteLine(stage.Key.ToEscapedString()!, " => [", string.Join(", ", publicKey), "],");
                        }

                        code.WriteLine("_ => throw new ", code.GetName<NotImplementedException>(), "()")
                            .EndSwitch();
                    })
                .ToFile(path, default);
        }
    }
}
