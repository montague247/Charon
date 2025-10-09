using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel2 : SecurityLevel
{
    private const int ProviderKeySize = 1024;
    private readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

    public SecurityLevel2() : base(2)
    {
    }

    protected override byte[] EncryptValue(string value, byte[] key)
    {
        throw new NotImplementedException();
    }

    protected override string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash)
    {
        using var crypto = new RSACryptoServiceProvider(ProviderKeySize);
        crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

        var salted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
        var decrypted = RemoveSalt(salted, out _);

        return Encoding.UTF8.GetString(decrypted);
    }
}
