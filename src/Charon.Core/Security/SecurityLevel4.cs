using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel4 : SecurityLevel
{
    private const int ProviderKeySize = 2048;
    private const int DeriveBytesIterations = 50000;
    private const int DerivedBytesLength = 128;
    private readonly HashAlgorithmName DeriveBytesHashAlgorithmName = HashAlgorithmName.SHA512;
    private readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

    public SecurityLevel4() : base(4)
    {
    }

    protected override string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash)
    {
        return DecryptValue(encrypted, key, hash, ProviderKeySize, SecureEncryptionPadding, DeriveBytesIterations, DeriveBytesHashAlgorithmName, DerivedBytesLength);
    }
}
