using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public sealed class SecurityLevel5 : SecurityLevel
{
    public SecurityLevel5() : base(5)
    {
    }

    protected override int KeySize => 2048;

    protected override int DeriveBytesIterations => 100000;

    protected override HashAlgorithmName DeriveBytesHashAlgorithmName => HashAlgorithmName.SHA512;

    protected override int DerivedBytesLength => 128;

    protected override byte[] EncryptValue(string value, byte[] key)
    {
        using var crypto = new RSACryptoServiceProvider(KeySize);

        crypto.ImportCspBlob(key);

        var salt = CreateSalt();
        var salted = new List<byte>(salt);
        var derivedBytes = Rfc2898DeriveBytes.Pbkdf2(salt.SecureHash().Veil(salt), salt, DeriveBytesIterations, DeriveBytesHashAlgorithmName, DerivedBytesLength);

        salted.AddRange(Encoding.UTF8.GetBytes(value).Veil(derivedBytes));

        return crypto.Encrypt(salted.ToArray(), EncryptionPadding);
    }
}
