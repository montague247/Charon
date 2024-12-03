using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Charon.Types;
using Serilog;

namespace Charon.Security
{
    public static partial class SecureEncrypt
    {
        public const int MaxLength = 97;
        private const int SaltLength = 127 - MaxLength;
        private const int DeriveBytesIterations = 50000;
        private static readonly HashAlgorithmName DeriveBytesHashAlgorithmName = HashAlgorithmName.SHA512; // change to SHA3_512 if supported on Mac
        private const int DerivedBytesLength = 128;
        private const string SecurePrefix = "{enc:sl4}";
        private const string InsecurePrefix1 = "{enc:sl2}";
        private const string InsecurePrefix2 = "{enc:sl3}";
        private static readonly RSAEncryptionPadding SecureEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

        public static IPrivateKeyRetriever? PrivateKeyRetriever { get; set; }

        public static IPublicKeyRetriever? PublicKeyRetriever { get; set; }

        public static bool IsEncrypted(this string? value)
        {
            return !string.IsNullOrEmpty(value) &&
                    (
                        value.StartsWith(SecurePrefix, StringComparison.Ordinal) ||
                        value.StartsWith(InsecurePrefix2, StringComparison.Ordinal) ||
                        value.StartsWith(InsecurePrefix1, StringComparison.Ordinal)
                    );
        }

        public static bool IsSecureEncrypted(this string? value)
        {
            return !string.IsNullOrEmpty(value) &&
                value.StartsWith(SecurePrefix, StringComparison.Ordinal);
        }

        public static string Encrypt(string value, string? stage)
        {
            var retriever = PublicKeyRetriever ??= FindKeyRetriever<IPublicKeyRetriever>();

            return Encrypt(value, stage, retriever);
        }

        public static string Encrypt(string value, string? stage, IPublicKeyRetriever retriever)
        {
            var key = retriever.GetKey(stage ?? retriever.DefaultStage);

            if (key == null)
                return value;

            if (value.Length <= MaxLength)
                return EncryptValue(value, key);

            var sb = new StringBuilder();

            do
            {
                if (sb.Length > 0)
                    sb.Append('|');

                if (value.Length <= MaxLength)
                {
                    sb.Append(EncryptValue(value, key));
                    break;
                }
                else
                    sb.Append(EncryptValue(value[..MaxLength], key));

                value = value[MaxLength..];
            } while (true);

            return sb.ToString();
        }

        public static string Decrypt(string value)
        {
            var retriever = PrivateKeyRetriever ??= FindKeyRetriever<IPrivateKeyRetriever>();

            return Decrypt(value, retriever);
        }

        public static string Decrypt(string value, IPrivateKeyRetriever retriever)
        {
            if (!IsEncrypted(value))
                return value;

            var key = retriever.GetKey();

            if (key == null)
                return value;

            var hash = retriever.GetHash();
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

        private static string DecryptValue(string value, byte[] key, byte[]? hash)
        {
            using var crypto = new RSACryptoServiceProvider(1024);
            crypto.ImportCspBlob(hash == null ? key : key.Unveil(hash.Secure()!));

            if (value.StartsWith(SecurePrefix, StringComparison.Ordinal))
                return DecryptSecure(crypto, Convert.FromBase64String(value[SecurePrefix.Length..]));
            else if (value.StartsWith(InsecurePrefix2))
                return DecryptInsecure2(crypto, Convert.FromBase64String(value[InsecurePrefix2.Length..]));
            else
                return DecryptInsecure1(crypto, Convert.FromBase64String(value[InsecurePrefix1.Length..]));
        }

        private static string DecryptSecure(RSA crypto, byte[] encrypted)
        {
            var decrypted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
            var salt = GetSalt(decrypted);
            decrypted = RemoveSalt(decrypted);

            var derivedBytes = new Rfc2898DeriveBytes(salt.SecureHash().Veil(salt), salt, DeriveBytesIterations, DeriveBytesHashAlgorithmName).GetBytes(DerivedBytesLength);

            return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
        }

        private static string DecryptInsecure1(RSA crypto, byte[] encrypted)
        {
            var salted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
            var decrypted = RemoveSalt(salted);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static string DecryptInsecure2(RSA crypto, byte[] encrypted)
        {
            var decrypted = crypto.Decrypt(encrypted, SecureEncryptionPadding);
            var salt = GetSalt(decrypted);
            decrypted = RemoveSalt(decrypted);

#pragma warning disable SYSLIB0041 // Typ oder Element ist veraltet
            var derivedBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(salt), salt, 50000).GetBytes(128);
#pragma warning restore SYSLIB0041 // Typ oder Element ist veraltet

            return Encoding.UTF8.GetString(decrypted.Unveil(derivedBytes));
        }

        private static byte[] GetSalt(byte[] bytes)
        {
            var salt = new byte[SaltLength];

            Buffer.BlockCopy(bytes, 0, salt, 0, SaltLength);

            return salt;
        }

        private static byte[] RemoveSalt(byte[] bytes)
        {
            var size = bytes.Length - SaltLength;
            var none = new byte[size];

            Buffer.BlockCopy(bytes, SaltLength, none, 0, size);

            return none;
        }

        private static string EncryptValue(string value, byte[] key)
        {
            byte[] encrypted;

            using (var crypto = new RSACryptoServiceProvider(1024))
            {
                crypto.ImportCspBlob(key);

                var salt = SecurityExtensions.CreateRandom(SaltLength);
                var salted = new List<byte>(salt);
                var derivedBytes = new Rfc2898DeriveBytes(salt.SecureHash().Veil(salt), salt, DeriveBytesIterations, DeriveBytesHashAlgorithmName).GetBytes(DerivedBytesLength);

                salted.AddRange(Encoding.UTF8.GetBytes(value).Veil(derivedBytes));

                encrypted = crypto.Encrypt(salted.ToArray(), SecureEncryptionPadding);
            }

            return string.Concat(SecurePrefix, Convert.ToBase64String(encrypted));
        }

        private static T FindKeyRetriever<T>()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsPublic && !t.IsInterface && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
                .OrderByDescending(s => s.GetCustomAttribute<PriorityAttribute>()?.Priority ?? int.MinValue)
                .ToArray();

            var type = types[0];

            Log.Information("Found '{TypeName}' as implementation for '{SearchTypeName}'", type.FullName, typeof(T).FullName);

            return (T)Activator.CreateInstance(type)!;
        }
    }
}
