using System.Security.Cryptography;
using System.Text;
using Charon.Security;

namespace Charon;

public static class SecurityExtensions
{
    public static string? Encrypt(this string? value, string? stage = null)
    {
        if (value == null)
            return value;

        return SecureEncrypt.Encrypt(value, stage);
    }

    public static string? Decrypt(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return SecureEncrypt.Decrypt(value);
    }

    public static byte[] CreateRandom(int length = 256)
    {
        var bytes = new byte[length];

        RandomNumberGenerator.Fill(bytes);

        return bytes;
    }

    public static string? Secure(this string? value, byte[] secureHash)
    {
        if (value == null)
            return default;

        return value.Encrypt()!.Veil(secureHash);
    }

    public static string? Secure(this string? value, string secret)
    {
        if (value == null)
            return default;

        return value.Encrypt()!.Veil(secret.SecureHash());
    }

    public static byte[]? Secure(this byte[]? bytes)
    {
        if (bytes == null ||
            bytes.Length == 0)
            return bytes;
        if (bytes.Length % 2 != 0)
            throw new ArgumentException($"Length of byte array must be even");

        var length = bytes.Length / 2;
        var salt = new byte[length];

        Buffer.BlockCopy(bytes, 0, salt, 0, length);

        var hash = new byte[length];

        Buffer.BlockCopy(bytes, length, hash, 0, length);

        return Secure(hash, salt);
    }

    public static byte[] Secure(this byte[] hash, byte[] salt)
    {
        if (hash.Length == 0 ||
            salt.Length == 0)
            return hash;

        var derivedBytes = new Rfc2898DeriveBytes(hash.SecureHash().Veil(salt), salt, 50000, HashAlgorithmName.SHA512).GetBytes(128);

        return derivedBytes.Veil(salt);
    }

    public static string? Unsecure(this string? value, byte[] secureHash)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Unveil(secureHash).Decrypt();
    }

    public static string? Unsecure(this string? value, string secret)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Unveil(secret.SecureHash()).Decrypt();
    }

    public static string? Veil(this string? value, byte[] secureHash)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Convert.ToBase64String(Veil(Encoding.UTF8.GetBytes(value), secureHash, false));
    }

    public static string? Veil(this string? value, string secret)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Veil(value, SecureHash(secret));
    }

    public static byte[] Veil(this byte[] inBytes, byte[] secureHash, bool clone = true)
    {
        var hashIdx = 0;
        var outBytes = clone ? new byte[inBytes.Length] : inBytes;

        for (int idx = 0; idx < inBytes.Length; idx++)
        {
            var b = inBytes[idx];
            var h = secureHash[hashIdx++];

            int i = (int)b + h;

            if (i > 255)
                i -= 256;

            outBytes[idx] = (byte)i;

            if (hashIdx == secureHash.Length)
                hashIdx = 0;
        }

        return outBytes;
    }

    public static string? Unveil(this string? value, byte[] secureHash)
    {
        if (value == null)
            return default;

        return Encoding.UTF8.GetString(Unveil(Convert.FromBase64String(value), secureHash, false));
    }

    public static string? Unveil(this string? value, string secret)
    {
        if (value == null)
            return default;

        return Unveil(value, SecureHash(secret));
    }

    public static byte[] Unveil(this byte[] inBytes, byte[] secureHash, bool clone = true)
    {
        var hashIdx = 0;
        var outBytes = clone ? new byte[inBytes.Length] : inBytes;

        for (int idx = 0; idx < inBytes.Length; idx++)
        {
            var b = inBytes[idx];
            var h = secureHash[hashIdx++];

            int i = (int)b - h;

            if (i < 0)
                i += 256;

            outBytes[idx] = (byte)i;

            if (hashIdx == secureHash.Length)
                hashIdx = 0;
        }

        return outBytes;
    }

    public static byte[] SecureHash(this byte[] value)
    {
        return SHA512.HashData(value);
    }

    public static byte[] SecureHash(this string value)
    {
        return SecureHash(Encoding.UTF8.GetBytes(value));
    }
}
