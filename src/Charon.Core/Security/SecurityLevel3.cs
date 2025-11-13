using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel3 : SecurityLevel
{
    private const int ProviderKeySize = 1024;

    public SecurityLevel3() : base(3)
    {
    }
    
    protected override int KeySize => throw new NotImplementedException();

    protected override int DeriveBytesIterations => 50000;

    protected override HashAlgorithmName DeriveBytesHashAlgorithmName => throw new NotImplementedException();

    protected override int DerivedBytesLength => 128;

    protected override string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash)
    {
        using var crypto = new RSACryptoServiceProvider(ProviderKeySize); // NOSONAR

        crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

        var decrypted = crypto.Decrypt(encrypted, EncryptionPadding);
        decrypted = RemoveSalt(decrypted, out byte[] salt);

        var derivedBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(salt), salt, DeriveBytesIterations).GetBytes(DerivedBytesLength); // NOSONAR

        return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
    }
}
