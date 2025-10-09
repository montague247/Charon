using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel4 : SecurityLevel
{
    private const int ProviderKeySize = 2048;
    private const int DeriveBytesIterations = 50000;
    private const int DerivedBytesLength = 128;
    private readonly HashAlgorithmName DeriveBytesHashAlgorithmName = HashAlgorithmName.SHA512; // create new level with SHA3_512 if supported on Mac
    private readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

    public SecurityLevel4() : base(4)
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
        using var crypto = new RSACryptoServiceProvider(ProviderKeySize);
        crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

        var decrypted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
        decrypted = RemoveSalt(decrypted, out byte[] salt);

        var derivedBytes = new Rfc2898DeriveBytes(salt.SecureHash().Veil(salt), salt, DeriveBytesIterations, DeriveBytesHashAlgorithmName).GetBytes(DerivedBytesLength);

        return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
    }
}
