using System.Diagnostics;
using Charon.Security;

namespace Charon.Core.Tests.Security
{
    public sealed class SecureEncryptTests
    {
        private static readonly string[] EncryptedValues = [
            "{enc:sl5}jN80hNJWum4Kj3tyF4Wgl1iYwXMQuUUog8S8fkpBo0qMntD2ftzaI0vIJ3DVvFaDdU6VFZhcWDNvTf8G7CYcUIobdfRiO0+s3SvgLseBpJa+PKoClcXRLK6kfGfo/Ni85caraJcZ3IjCFVZ/cBeOar7xKfUqe881k3Tid98qqPsGZWX84dWjZHXpEOTegAXzqIDXuvQQH4l1jT1mfJl3JIOm3SkAmCNgo4EzTc9/YVDTHLvfal+zeWl8knuCd6eQXvQkfhfZp9wtwO0HhrmtMrll+eVI2Uqf1an0l7WqMqsQ4n3SBpgYxcbHU+KUfMMJrufd7yj5Hxbo3CIK+Vxnus6LNsBhBbgHNXdbMJcSMo5VwVobdw2BKh7eMl1HaPmBTnq5/MuFDq1efhW4m7JZ4OY7dNbdMydz1aUXBqyWeF3vk/5klJcaLEGiEWS/L5HPWPuuV0Xx+3xQGntF0YDPEDcV+f0xFitXJNzA6DibEZ6/aZIjnjTzpKqg0567SANLuhLQyZNEaqsrSdlvMrf3aCv+mziybFpV7CnIlw0PUNIdTiOaCVIMJuJ/ysOqafKmjJlg28qpvlqkQJr5K0NT+7GpgKhOV3m9y/KgXXWGqq5tDwAwYvT9j9cCNVqIGctC8mzSCWBVwnrj/G1zKpLGkReK+C54glMiJOpsVejWCN0=", // abc
            "{enc:sl5}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" // abcde...
        ];

        [Fact]
        public void IsEncrypted()
        {
            Assert.False(SecureEncrypt.IsEncrypted(string.Empty));
            Assert.False(SecureEncrypt.IsEncrypted("foo"));
            Assert.True(SecureEncrypt.IsEncrypted("{enc:sl5}"));
            // Assert.True(SecureEncrypt.IsEncrypted("{enc:sl4}"));
            // Assert.True(SecureEncrypt.IsEncrypted("{enc:sl3}"));
            // Assert.True(SecureEncrypt.IsEncrypted("{enc:sl2}"));
        }

        [Fact]
        public void IsSecureEncrypted()
        {
            Assert.False(SecureEncrypt.IsSecureEncrypted(string.Empty));
            Assert.False(SecureEncrypt.IsSecureEncrypted("foo"));
            Assert.True(SecureEncrypt.IsSecureEncrypted("{enc:sl5}"));
            // Assert.False(SecureEncrypt.IsSecureEncrypted("{enc:sl4}"));
            // Assert.False(SecureEncrypt.IsSecureEncrypted("{enc:sl3}"));
            // Assert.False(SecureEncrypt.IsSecureEncrypted("{enc:sl2}"));
        }

        [Fact]
        public void EncryptSecure()
        {
            var actual = SecureEncrypt.Encrypt("abc", null);
            Debug.WriteLine(actual);
            Assert.StartsWith("{enc:sl5}", actual);
            Assert.True(actual.IsSecureEncrypted());
            Assert.Equal(693, actual.Length);

            var bytes = Convert.FromBase64String(actual[9..]);
            Assert.NotNull(bytes);
            Assert.Equal(512, bytes.Length);

            foreach (var encryptedValue in EncryptedValues)
            {
                Assert.NotEqual(encryptedValue, actual); // should never again create the same encrypted value

                if (!encryptedValue.Contains('|'))
                    Assert.True(actual.Length >= encryptedValue.Length);
            }
        }

        [Fact]
        public void DecryptSecure()
        {
            var encrypted = EncryptedValues[0];
            var actual = SecureEncrypt.Decrypt(encrypted);

            Assert.Equal("abc", actual);
        }

        [Fact]
        public void EncryptInsecure()
        {
            for (int i = 1; i < SecureEncrypt.Levels.Length; i++)
            {
                var securityLevel = SecureEncrypt.Levels[i];

                Assert.Throws<NotImplementedException>(() => securityLevel.Encrypt("abc", []));
            }
        }

        [Fact]
        public void DecryptInsecure()
        {
            var retriever = new NullHashPrivateKeyRetriever();

            for (int i = 1; i < SecureEncrypt.Levels.Length; i++)
            {
                var securityLevel = SecureEncrypt.Levels[i];

                var encrypted = EncryptedValues.Single(securityLevel.Match);
                var actual = SecureEncrypt.Decrypt(encrypted, retriever);

                Assert.Equal("abc", actual);
            }
        }

        [Fact]
        public void DecryptNullPrivateKeyRetriever()
        {
            var encrypted = EncryptedValues[0];
            var actual = SecureEncrypt.Decrypt(encrypted, new NullPrivateKeyRetriever());

            Assert.Equal(encrypted, actual);
        }

        [Fact]
        public void EncryptNullHashPrivateKeyRetriever()
        {
            var plain = "abc";
            var actual = SecureEncrypt.Encrypt(plain, null, new NullHashPublicKeyRetriever());

            Assert.True(true, actual);
        }

        [Fact]
        public void DecryptNullHashPrivateKeyRetriever()
        {
            var encrypted = "{enc:sl4}Jbd5XqdCi3YnZ1rMR35Xne7GKF8gS6fKtA9tPxn+JreRVKsSLiN5EhSgSIOJH9swNjreVmh7nTee090dpBQ0tR77zL/1k8F1cqLQmYG1pGcRRywfvFm2hjLzrztM4tNIzBZkAJGGF3TzMxW/mOArLPzCthgB9ms/UB35jCH6cgiIg/sg4tMhn5TOwvTNE4ZAUI2Epof1Up3BQ/pSpsb8CJlMjBtWaiW7nemEF1cfGPpXKRCJA/ccYl6/pliKLWaEi6/IoGE551ejkle0kDOoyJ+DKUnVR3vSUlSLaTn4qRTPoTlNgCGoJq7ucGpv0Kyo/83MqipShNcyTjrM1RaIOc08hlbyIM0yq0le90lTzGlqeo7MQEdI0b0dhCIIcmT9ZStn0DpESTWGaZ1bjg+sFWS5vmBEqGgwXaj+hqZ+O+1RM0/UyFqHlrYu8fYbGh/YM1xt1+ANioYJ2DjKIJ2WrpJzHYeX97fJTK/aroHMCbVheFVBe4u1kr9lhHjQX4P353huuEZJ48ozzC/Ttr0tnVEmIL2yBsS2HbbnFocLhZz+hg622GFj+QW0CGcPnRb+R5/Pa4XwtnqX+GFejhgT6BaDNfmfmNujk70fyEX0j2R3LcYksrUikTLIH9UdNV7cykpdU9Y/Y0Ab9sf+1PLcAaZDCGq1cuzdw/oIQMcX/9c=";
            var actual = SecureEncrypt.Decrypt(encrypted, new NullHashPrivateKeyRetriever());

            Assert.Equal("abc", actual);
        }

        [Fact]
        public void DecryptNotEncrypted()
        {
            var encrypted = "foo";
            var actual = SecureEncrypt.Decrypt(encrypted);

            Assert.Equal("foo", actual);
        }

        [Fact]
        public void EncryptVeryLong()
        {
            var actual = SecureEncrypt.Encrypt("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\", null);
            Assert.StartsWith("{enc:sl5}", actual);
            Assert.True(actual.IsSecureEncrypted());
            Debug.WriteLine(actual);
            Assert.Equal(2063, actual.Length);

            var parts = actual.Split('|');
            Assert.Equal(3, parts.Length);

            foreach (var part in parts)
            {
                var base64 = part.StartsWith("{enc:sl", StringComparison.Ordinal) ? part[(part.IndexOf('}') + 1)..] : part;
                var bytes = Convert.FromBase64String(base64);
                Assert.NotNull(bytes);
                Assert.Equal(512, bytes.Length);
            }

            foreach (var encryptedValue in EncryptedValues)
            {
                Assert.NotEqual(encryptedValue, actual); // should never again create the same encrypted value
            }
        }

        [Fact]
        public void DecryptVeryLong()
        {
            var encrypted = EncryptedValues[1];
            var actual = SecureEncrypt.Decrypt(encrypted);

            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\", actual);
        }

        [Fact]
        public void DecryptVeryLongShortend()
        {
            var encrypted = EncryptedValues[2];
            var actual = SecureEncrypt.Decrypt(encrypted);

            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890!\"§$%&/()=?`´ß+ü*Ü#äö'ÄÖ@-.,_:;<>€^°\\", actual);
        }

        [Fact]
        public void EncryptNullPublicKeyRetriever()
        {
            var actual = SecureEncrypt.Encrypt("abc", null, new NullPublicKeyRetriever());
            Assert.Equal("abc", actual);
            Assert.False(actual.IsSecureEncrypted());
            Assert.False(actual.IsEncrypted());
        }

        [Fact]
        public void CheckEncryptedValues()
        {
            var retriever = new NullHashPrivateKeyRetriever();

            foreach (var encryptedValue in EncryptedValues)
            {
                var decrypted = SecureEncrypt.IsSecureEncrypted(encryptedValue) ?
                    SecureEncrypt.Decrypt(encryptedValue) : SecureEncrypt.Decrypt(encryptedValue, retriever);
                Assert.NotNull(decrypted);
                Assert.NotEmpty(decrypted);
            }
        }
    }
}
