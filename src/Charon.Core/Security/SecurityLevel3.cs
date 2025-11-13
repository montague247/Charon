using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel3 : SecurityLevel
{
    private const int ProviderKeySize = 1024;
    private const int DeriveBytesIterations = 50000;
    private const int DerivedBytesLength = 128;
    private readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

    public SecurityLevel3() : base(3)
    {
    }

    protected override string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash)
    {
        using var crypto = new RSACryptoServiceProvider(ProviderKeySize);
        crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

        var decrypted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
        decrypted = RemoveSalt(decrypted, out byte[] salt);

#pragma warning disable SYSLIB0041 // Typ oder Element ist veraltet
        var derivedBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(salt), salt, DeriveBytesIterations).GetBytes(DerivedBytesLength);
#pragma warning restore SYSLIB0041 // Typ oder Element ist veraltet

        return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
    }
}
