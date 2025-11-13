using System.Security.Cryptography;
using System.Text;

namespace Charon.Security;

public abstract class SecurityLevel(int level, int maxLength = 127, int saltLength = 30)
{
    public const int MinEncryptedValueLength = 9;
    private readonly int _saltLength = saltLength;
    private readonly int _maxValueLength = maxLength - saltLength;
    private readonly string _prefix = string.Concat("{enc:sl", level, "}");

    public bool Match(string value) => value.StartsWith(_prefix, StringComparison.Ordinal);

    public string Encrypt(string value, byte[] key, bool withPrefix = true)
    {
        if (value.Length <= _maxValueLength)
        {
            var encrypted = EncryptValue(value, key);
            var base64 = Convert.ToBase64String(encrypted);

            return withPrefix ? string.Concat(_prefix, base64) : base64;
        }

        var sb = new StringBuilder();

        do
        {
            if (sb.Length == 0)
                sb.Append(_prefix);
            else
                sb.Append('|');

            if (value.Length <= _maxValueLength)
            {
                sb.Append(Encrypt(value, key, false));

                break;
            }
            else
                sb.Append(Encrypt(value[.._maxValueLength], key, false));

            value = value[_maxValueLength..];
        } while (true);

        return sb.ToString();
    }

    public string Decrypt(string value, byte[] key, byte[]? hash)
    {
        var idx = value.IndexOf('|');

        if (idx == -1)
            return DecryptValue(value, key, hash);

        var sb = new StringBuilder();

        do
        {
            sb.Append(DecryptValue(value[..idx], key, hash));

            value = value[(idx + 1)..];
            idx = value.IndexOf('|');

            if (idx == -1)
            {
                sb.Append(DecryptValue(value, key, hash));

                break;
            }
        } while (true);

        return sb.ToString();
    }

    protected byte[] CreateSalt()
    {
        return SecurityExtensions.CreateRandom(_saltLength);
    }

    protected byte[] RemoveSalt(byte[] bytes, out byte[] salt)
    {
        salt = new byte[_saltLength];
        var size = bytes.Length - _saltLength;
        var none = new byte[size];

        Buffer.BlockCopy(bytes, 0, salt, 0, _saltLength);
        Buffer.BlockCopy(bytes, _saltLength, none, 0, size);

        return none;
    }

    protected string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash, int keySize, RSAEncryptionPadding encryptionPadding, int deriveBytesIterations, HashAlgorithmName deriveBytesHashAlgorithmName, int derivedBytesLength)
    {
        using var crypto = new RSACryptoServiceProvider(keySize);
        crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

        var decrypted = crypto.Decrypt(encrypted, encryptionPadding);
        decrypted = RemoveSalt(decrypted, out byte[] salt);

        var derivedBytes = new Rfc2898DeriveBytes(salt.SecureHash().Veil(salt), salt, deriveBytesIterations, deriveBytesHashAlgorithmName).GetBytes(derivedBytesLength);

        return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
    }

    protected virtual byte[] EncryptValue(string value, byte[] key)
    {
        throw new NotImplementedException();
    }

    protected abstract string DecryptValue(byte[] encrypted, byte[] key, byte[]? hash);

    private string DecryptValue(string value, byte[] key, byte[]? hash)
    {
        return DecryptValue(Convert.FromBase64String(value.StartsWith(_prefix, StringComparison.Ordinal) ? value[_prefix.Length..] : value), key, hash);
    }
}
