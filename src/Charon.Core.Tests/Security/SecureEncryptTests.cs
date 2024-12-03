using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Charon.Security;

namespace Charon.Core.Tests.Security
{
    public sealed class SecureEncryptTests
    {
        private static readonly string[] EncryptedValues = [
            "{enc:sl4}ZTai00kpLFe+9w/yGyKOFlu4lRvVXaSHGb7HUoYl7FFO5P1IU2YDaNXmKuu3sknBhbAKz3pDqkZUGDclfUaXRa8XHt25onGgPRsl8PC49IY8Tax67tX0aPDU3qTQdKRPN3w+wEKp4bLBlRttPHlvJmhgEl7wGp+Ts34eeUFQ/oGdiuyZeAam/TA9iGkFI3PIldJT9k8QiyIKt6LPeNuEEsw8L3MHgy4NSZZQ2eCyqcEIXdkr2HuHk/gfCoQ36LaLroj8ZS4q7F/ctpjavxe39uPdvXRxNTT5mH3lGnvcTTiA/cPOcUDgKR2+4DEf4L+AVBLr60p9fZHYH+2B/qrc0XDU3OCedEP+6wiJm2UVKFzQhxpk5HYT5APkEpMZ5VY1AS5e2Ntxwydb3wxfG1QUvDhzAIS6UaGVCXrQV43ezUsVdw0S3kBfuUBxVqzwz4FTzNOUCoPoVtW1KmL+xENLg7CGeIVE+2gLtKaEWFcVhsh6RGZe/fq9W6kzdWwF9k593FU/VtJCfA6KHgrZKRxx35mdMij92JEYY6zjVP05TjvF08l3wlvInnXWSjBc74u/9oHHzC49PNBfDEral8YLGfG393n5Ff4RW5VbCG/XuAg4I0839IeyqcHTU+kv97QdqpN7MBDQqb2CkC9UGPNUJpkDGdMT1vfplych95IHMfg=",
            "{enc:sl4}lp1z0rh1DeO1yDz+0LnybvrWUrFcTMzPA0Y16bTR0rIi/7ER1OilM+Z/biLaWfHDbfUvavlHvGECbTVw6bZjenK3qf4Cq+B+4OQSdxmB7N7C+/EwkdmogDe2E/AE24xE8YLS/ZyH7WhZgegELgMYlbudoRdOOAJp4rGP9yqAMItff/qaUALxtmjXMqJhxqX/E7MvgsBBsndhSmG61irp3wtNcuUjXroOZru+Tcc+VjgcpJRtyDDsTpXESpjR0Tg66hJ5cfrF8AEK4GttaAH3zmsT/0lUggFo+Kc/naI+ENAZElF3oAaFSpZOwFNpiQFpzW9+mK9IC1yBeQV1e8bFXbB+FpzHlNOFzNx67gQ6NYt8ZfvTyh75j6wPFGd1zy9UfOaUqU5CHXAhbmtqLpDOy8DMMBiVgOAMSkoonJ/Dr512SAV266US1blXy7c/iFQow2Vowspy39x0aALVYugqKuP4c41ys2pbhLntR6wM9rVwhscQqrrHtghk4tqKIkYStclRT/VayV8FXaQWMoQVHR0fQLEMT9zYqq5/CfJ+l1RZGCIZ7X61i212CH9nbeuuSIWP8JSCYgaRa33zDFH+ckjScySkqTKYLcf5ki5DiEXT6i8C1wOVfO2fPXIT8NzpHUI0Da+QaEE6c3dGjwnvaypJmxVNSC15mL0JPnAp6mQ=|{enc:sl4}YvtdR7lTN9Ln+DR7nvtrcr+F2x/A3yw1Jczh9sYywoSegcgZJIDNE/4OYQjg5y0Fr33KZ4Wv8whOEpsRw8BnmczddqUu7No9RyD2WcnbxolKz1Rbh+s31loyM17suzLqlBd+widOPohMocCbMAlWuOojXDplIfKOEHkCUB7x9V0Kka4mDtg/kJPTaFLPjTtN0p3nXvCXILZX1VvKiwahy0uGVjbBnVlluQqZEip7XWaePLb8bFEIp235M6tjvqjo633N19iEYQuicXT/MOMno2gTh9t6AYitAstAP9zbi31XuHIQCbxLeHdCdndlGXNtLCyg4DFLpiBEKs8/Epg8P3x7CaGG2C7SdvPMIgYBRtrVcmXutWQFYoVlEZnfCVNLoUhzPm6ea3mGVqWZAhWlNXoJqtMiIJKOCpk4/KL9XRWOi/2KuiSv1CnAWfrJig+0VmmkcDY8/4p+SqlXk48lrjuJo4OM7S8UK+RxZgv51nFAt0d9FsDDyYj1GG4ZUCK3WFs4MDdyNaOUNAdHokf69zgr+TdACdXQ9t9qVM0jOzLjEPsLEMJdupaLtmaO+721uvMZcEAsg78BXG2jR4ltbNbRUMhYFs5tgnGiU3/kd9qexrZ3DoEmb4DO1mYdkO/B2bDKwRivesbmPpFHvIj0lxbyS3HQI95Ixm4gq71BCD0=|{enc:sl4}fxpYuTsZnqn/EU5uGB0v2DtdlHGVQ9px4zh8nRXjBr217IbF3zKSeg1aPve8v83LBza5toIZeiloxPspqoEn4I2m/h0EAMouxeKfS5GN3hizGriGRL52mxBLIX0gEmvWC3r+72jDe+/YVdfTxxTPyueyyODTsdfuqp2MRvHS8pQ4Gh9CRiiMXUXaKZRm9GIb14NS2qH+LnpS4kTt85Xb2a2QbzlC+sNzl8shTY9KrL1qXV5vflq7xde66FRUeOcTThjCCi7UWSyAhw+yZ2QXO1eyBMpRIXxYZSozUnWmAk36m5g79QddMoIaFPtl6mflcFKevrUUILW/d+j6K9nFGoYnvPuX56x8fUYcBYf+dfOUFKzOFS3jmSBwtF1YbvcVOBDTTZtCNsNXGX2JPEd7qpKo/F0e+o0e4VJL+8+B1KS5f1qIuRD+PgMDi+FB3ARKr3lenuEwncrXymLSW6XsK/i/JDE7/9I4H1tl6UlxvkaYeZ4pYu640kc+wOcmiYnARh421GmwcRx8VnX6h6ixUaW2LgtuCAPAZz3fujWk/J6r45VTl+0efnbnLr53ZJs64q0yCxjRu3c7QJ8+G6cqn91QO+N2hd1L+pRBZtcK6CaFy7dvPQ0RjDLEBcLrbCAPFUSBaLN+5hiNbC9fctLa0UNxVYbIPthqjne7LhekbuY=",
            "{enc:sl2}EdPqlpAWdiJCKIv8R+wkwei7HEpsvcoXQp8LPoLS0zFSKLVr/F1lh3e7kpcR8rP1kRGibzGvMP2pPoRqe2H/wszA58LAV1+XzV5bPRv1pGzk47x/2+2YM4G6du9YDTDKcwyI2iXuIaiXiQrJvcrVd0mKQSjqopQTGhlzW6qbHkySNjEK3i1n8hN6IVt7gh8J+YLlgqIHccFImAfhq2h3BALkyp5yj5VonR/FJ2Qd4fSh5TA1RWmaYPkhLjo+1Ym1o+8K7WB3TIYI/XXNK+lPSl5Oc3tcksRD1s5eUVSfmJ/agFkA/EdpGzNjkwP9+hHH1QYub9rXy5Zf4UHh9yuEqI0+trFBwHph1aKcOsr6mik31MgHkbqkKdoGDYzslRi/TTLiRXP2NrIYNeu1qCOCL7C7H5w39wfV+qQvs5XkKgjd9yxWq43auCJUSr67D3t+a3ljoB1rTrijn2Z07UCjPN0BqpoBMxWCRHMNkJd5WsnKrWf32g/E4cuTBYlL8Gx+ON2tCEuJL9mNHMOdFvmVGggxEwxDIYDaSk7zFL9A9YC+xMSBrna36DD9EqkCXqTIaMgBbfg7girbYfMtYTGrqZJ2KeAPiR8tTBd3PQzrdUGAt466TNXD88A8FB5C/oiTSV2gXqDR+QfF2SpqbMMAaUiW6e9CiwRQy3C+Obu5sk4=",
            "{enc:sl3}tO0JcZ3N4jGhGpTirQPb9SSOfaHPo1rdA+VZK8QYjpeN2GQenGnMBR8XB8wFCOSWb1njTdgYvW/NxAigmPs+84KPn7ZTs4R7GDhDEPj3k4h70IWgUnmBwsCEFC68luzE48AObqRMvjUn5itvzLj/5M/+Y5Vf2grpHpG6qZzNny3CxGN2X7FO/i5ChXpgbJfBxbUqAn2Wra7aWPrlrkd7TwH0CPe2vaiu3BN2pDWO6QTQYtbJGHP1l9nOUUcYZZYuBLePinK+1B468mRvLi/LF20uzS3OkI/jPAqtKGmwJRQz2AnAJsnVVZB6sGBwzet8fF02EaE7KZPRul4nxIQrGw8yuBNEJqpuAZxWThkPC/Ydmf4Ip9CUoT1Q6Sjpm6c69IztFRzjzLex3rObGDs/rtHCaF1CvjIaAt2Aax/ptbgi59Ef2k0x/gNThU3ljJFlM+IJqjZTISG1Jzp3ShVgdWXo+5SMyQ2dUgrR8VCZfqUVhYiav7/1CXoa7ju8rH9qR9ccUMkTpy2w1mGIyzOc4P8m4gzAXJPos2s3u9n+DnUms5yRG/Zr6lmPvhsMkRw9dLuC+aduvZLHDD/iCU0bUCKp9KlXz/1OOo6m+/WxqDgm92tQF1NLvqIYY8L7WoIcUIzOSp9WnMo9ZUNMpLR6JZCMYbgBWBSNRLBZIxhIhjA="
        ];

        [Fact]
        public void IsEncrypted()
        {
            Assert.False(SecureEncrypt.IsEncrypted(string.Empty));
            Assert.False(SecureEncrypt.IsEncrypted("foo"));
            Assert.True(SecureEncrypt.IsEncrypted("{enc:sl4}"));
            Assert.True(SecureEncrypt.IsEncrypted("{enc:sl3}"));
            Assert.True(SecureEncrypt.IsEncrypted("{enc:sl2}"));
        }

        [Fact]
        public void IsSecureEncrypted()
        {
            Assert.False(SecureEncrypt.IsSecureEncrypted(string.Empty));
            Assert.False(SecureEncrypt.IsSecureEncrypted("foo"));
            Assert.True(SecureEncrypt.IsSecureEncrypted("{enc:sl4}"));
            Assert.False(SecureEncrypt.IsSecureEncrypted("{enc:sl3}"));
            Assert.False(SecureEncrypt.IsSecureEncrypted("{enc:sl2}"));
        }

        [Fact]
        public void Encrypt()
        {
            var actual = SecureEncrypt.Encrypt("abc", null);
            Debug.WriteLine(actual);
            Assert.StartsWith("{enc:sl4}", actual);
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
        public void Decrypt()
        {
            var encrypted = EncryptedValues[0];
            var actual = SecureEncrypt.Decrypt(encrypted);

            Assert.Equal("abc", actual);
        }

        [Fact]
        public void EncryptSL2()
        {
            var key = new NullHashPublicKeyRetriever().GetKey("TESTS")!;
            using var crypto = new RSACryptoServiceProvider(1024);
            crypto.ImportCspBlob(key);
            var salt = new byte[30];
            var salted = new List<byte>(salt);
            salted.AddRange(Encoding.UTF8.GetBytes("abc"));

            var encrypted = crypto.Encrypt(salted.ToArray(), RSAEncryptionPadding.OaepSHA1);
            var str = string.Concat("{enc:sl2}", Convert.ToBase64String(encrypted));
            Assert.True(true, str);
        }

        [Fact]
        public void DecryptSL2()
        {
            var encrypted = EncryptedValues[2];
            var actual = SecureEncrypt.Decrypt(encrypted, new NullHashPrivateKeyRetriever());

            Assert.Equal("abc", actual);
        }

        [Fact]
        public void EncryptSL3()
        {
            var key = new NullHashPublicKeyRetriever().GetKey("TESTS")!;
            using var crypto = new RSACryptoServiceProvider(1024);
            crypto.ImportCspBlob(key);

            var salt = new byte[30];

            RandomNumberGenerator.Fill(salt);

            var salted = new List<byte>(salt);
#pragma warning disable SYSLIB0041 // Typ oder Element ist veraltet
            var derivedBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(salt), salt, 50000).GetBytes(128);
#pragma warning restore SYSLIB0041 // Typ oder Element ist veraltet

            salted.AddRange(Encoding.UTF8.GetBytes("abc").Veil(derivedBytes));

            var encrypted = crypto.Encrypt(salted.ToArray(), RSAEncryptionPadding.OaepSHA1);
            var str = string.Concat("{enc:sl3}", Convert.ToBase64String(encrypted));
            Assert.True(true, str);
        }

        [Fact]
        public void DecryptSL3()
        {
            var encrypted = EncryptedValues[3];
            var actual = SecureEncrypt.Decrypt(encrypted, new NullHashPrivateKeyRetriever());

            Assert.Equal("abc", actual);
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
            Assert.StartsWith("{enc:sl4}", actual);
            Assert.True(actual.IsSecureEncrypted());
            Debug.WriteLine(actual);
            Assert.Equal(2081, actual.Length);

            var parts = actual.Split('|');
            Assert.Equal(3, parts.Length);

            foreach (var part in parts)
            {
                var bytes = Convert.FromBase64String(part[9..]);
                Assert.NotNull(bytes);
                Assert.Equal(512, bytes.Length);
            }

            foreach (var encryptedValue in EncryptedValues)
            {
                Assert.NotEqual(encryptedValue, actual); // should never again create the same encrypted value
                Assert.True(actual.Length >= encryptedValue.Length);
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
