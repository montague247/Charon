using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel5 : SecurityLevel
{
    private const int ProviderKeySize = 2048;
    private const int DeriveBytesIterations = 100000;
    private const int DerivedBytesLength = 128;
    private readonly HashAlgorithmName DeriveBytesHashAlgorithmName = HashAlgorithmName.SHA512; // create new level with SHA3_512 if supported on Mac
    private readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

    public SecurityLevel5() : base(5)
    {
    }

    protected override byte[] EncryptValue(string value, byte[] key)
    {
        using var crypto = new RSACryptoServiceProvider(ProviderKeySize);

        crypto.ImportCspBlob(key);

        var salt = CreateSalt();
        var salted = new List<byte>(salt);
        var derivedBytes = new Rfc2898DeriveBytes(salt.SecureHash().Veil(salt), salt, DeriveBytesIterations, DeriveBytesHashAlgorithmName).GetBytes(DerivedBytesLength);

        salted.AddRange(Encoding.UTF8.GetBytes(value).Veil(derivedBytes));

        return crypto.Encrypt(salted.ToArray(), SecureEncryptionPadding);
    }

    protected override string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash)
    {
        return DecryptValue(encrypted, key, hash, ProviderKeySize, SecureEncryptionPadding, DeriveBytesIterations, DeriveBytesHashAlgorithmName, DerivedBytesLength);      
    }
}
