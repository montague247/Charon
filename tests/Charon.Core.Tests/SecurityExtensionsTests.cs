using Charon.Security;

namespace Charon.Core.Tests
{
    public sealed class SecurityExtensionsTests
    {
        [Fact]
        public void Encrypt()
        {
            var actual = "abc".Encrypt();
            Assert.True(actual.IsSecureEncrypted());

            actual = "abc".Encrypt("TESTS");
            Assert.True(actual.IsSecureEncrypted());

            actual = string.Empty.Encrypt();
            Assert.True(actual.IsSecureEncrypted());

            actual = null;
            actual = actual.Encrypt();
            Assert.Null(actual);
        }

        [Fact]
        public void Decrypt()
        {
            var actual = "abc".Encrypt().Decrypt();
            Assert.False(actual.IsEncrypted());
            Assert.Equal("abc", actual);

            actual = string.Empty.Encrypt().Decrypt();
            Assert.False(actual.IsEncrypted());
            Assert.Equal(string.Empty, actual);

            actual = null;
            actual = actual.Encrypt().Decrypt();
            Assert.False(actual.IsEncrypted());
            Assert.Null(actual);
        }

        [Fact]
        public void CreateRandom()
        {
            var actual = SecurityExtensions.CreateRandom();
            Assert.Equal(256, actual.Length);

            actual = SecurityExtensions.CreateRandom(16);
            Assert.Equal(16, actual.Length);
        }

        [Fact]
        public void SecureWithHash()
        {
            var actual = "abc".Secure([1, 2, 3, 4, 5]);
            Assert.NotNull(actual);

            actual = null;
            actual = actual.Secure([1, 2]);
            Assert.Null(actual);
        }

        [Fact]
        public void UnecureWithHash()
        {
            var secured = "fGdxZz90bjiBb086M2xTS1l4cTlMbDZ4fkc2WmtxMmtce11OU3hZWnBpO1c9Z21zRnQxc1ByeUU0aXh/Ykszek5LNUdae0djR2haN1hJXm1kWUdSe1VoO0s8RFtmWU5wZGdqV2pRMy94NFV5a1F0Z0V0T2ItU090RG5mXFdNTTlva0hocjNTajo4Z2ZzY01nXzRLbUdLV1wyZ0dmUWR2PHlNaVl2Zjo7NXA0VmxoPjlzdFR4SFxaXD01ZlpuX0lac0lUVWdqRV17c0xIXXZ4VFVNNW40blkyb2lOcTRMTFNyNFVuRXJEUGpzOUZ8V2c+MFtZSFlJTnlqZm0tfWlcbTpucnpEZjlpVll4VG9raWhddD54dnpTNUlqdXF5TnRvcDBmWEw2WnJoNGVzMW46W3ZOc3ZVOW81VkZ1aFt7Z2dJVy5PWmdPUE53dmhnO35rN0t8Z3A1Rk1QLFh7cnp0OE9SeENqRWZsSVBbaGdOTGZXUnA3WXtbcGRnezdDTWs7ak5uNExmUW9FWHNyNzJRekdGdDVqZ2paOHI4TF04VFo5Z1JnZU98aH8yY1hcR3J7WmlLNHhuMzpsbk1nZk1HSm1KWFUyUDpJUlpUenZYM1x9LDV7VUxvdkk0XkVSSEhoVy1pNH1Ha3dcT098RDpJamRIXjswY11Nb29sV351THNqNDo3OVZFU013a1BWelxRSWZydXVXaW14UHZrNGNGejBufGx9Z0dyWTtIb0tvezVRV1FNaVVrUmVIV0tQTnpLMXx3VHJjaU9ya0xvazc5c3N6cXJtVE53Nk0zUlksOUp0bExqUlo4bjt8M1BoWltbTHJzOHhJeEN6XXtVO209aERQWXVOSGV3Rz1ufFZHXENYenJ3azFKNX9Mck9LcFNnTi9INjZqcFJqTFJ0eFdnbVtITzJA";
            var actual = secured.Unsecure([1, 2, 3, 4, 5]);
            Assert.Equal("abc", actual);

            actual = string.Empty.Unsecure([1, 2]);
            Assert.Equal(string.Empty, actual);

            actual = null;
            actual = actual.Unsecure([1, 2]);
            Assert.Null(actual);
        }

        [Fact]
        public void SecureWithSecret()
        {
            var actual = "abc".Secure("Test");
            Assert.NotNull(actual);

            actual = null;
            actual = actual.Secure("Foo");
            Assert.Null(actual);
        }

        [Fact]
        public void UnsecureWithSecret()
        {
            var secured = "QVMMlgnP00oeO5Y1o19/0tsphxAEgDWSpAYcVwwxDgBOi3nFRyPdHWxAxm0zck84FDR70IQhf8HT/+8tnFS6pw840Xcl0q125TWdM8k9i/DnC14qL2poVnL3K1kIGiYGV3iHyl/9uPNwDOGQYlhXLRQ1lcBiA1bs5AXeC41X1WcxVOWZPou1ftkGq17lWHvr3udjRvxcWHhaIypl9CZPzm1fh81nE8AALkTBbmRwHQBDNpvSfg1i1dnN3wqlZdKJNjPthzTDqG0bQpFBy2yn2dX8TkjqgFZPmCYybNgPJuZiR3TjNxvLFmRIwl91YEcBAR6ho4v7V/MH+ufetXDhnf5ZDKgSwJ168im+Tt5dme7eJFNTLY1RTnMoWm0ZEk8DYT+Gzj/g3TljCu+IK3gbH0MSvNKI53mw9NvrGpSD7ZMOQ8l+JMK0YutDvWPXLqryufySPihJRWd26z54EzpQzUFilLpe8OovRSfmfl1ZLislErqUjSph7AP98uV9VvJoKzsKZBe9t4LjJbZuqCZ+/cr4iw0efGp1Xy0fSv/5LPAseG/aWvsBLHcK729TTlVBKSqRkq0eOrIMBcUWnFfPeC8z9Yb+qJxd8SiYcuhNYeD8301UCl1wknHwQUT1CiH6Sz+HqG708TxVI9yUPEIoMTQKpZVmFGXGCwflB7V71aItHtNpBq+oY+1GsEnEcIvWyRWLTytpZoqhDVpm2CYgDSCBm+FvEM44UxDRiER4W/giGpmomwd0zALc0fyIVf979WcRgkC9zWAOO5Jp2ilp+fQqhk0kZ0yQYAsYTvnwFN5le2zgRQS7NTZSvWViZDwfGTa8lKv3gr4Y8vEHdHXDlAk89KQYo8qJ5Am1d8Y6iMraK4hOI0VJT6ULWEzsMC/8QD9krSoV9BVmI92KbmJJMikImY90";
            var actual = secured.Unsecure("Test");
            Assert.Equal("abc", actual);

            actual = string.Empty.Unsecure("Foo");
            Assert.Equal(string.Empty, actual);

            actual = null;
            actual = actual.Unsecure("Foo");
            Assert.Null(actual);
        }

        [Fact]
        public void Secure()
        {
            var bytes = Array.Empty<byte>();
            var actual = bytes.Secure();
            Assert.NotNull(actual);
            Assert.Empty(actual);

            bytes = null;
            actual = bytes.Secure();
            Assert.Null(actual);

            bytes = [1, 2, 3];
            Assert.Throws<ArgumentException>(bytes.Secure);

            bytes = [1, 2, 3, 4];
            actual = bytes.Secure();
            Assert.NotNull(actual);
            Assert.Equal(128, actual.Length);

            byte[] expected = [96, 108, 67, 93, 185, 93, 26, 167, 81, 244, 52, 72, 146, 17, 1, 216, 151, 244, 177, 27, 142, 47, 107, 95, 7, 91, 141, 102, 61, 153, 222, 15, 234, 46, 216, 242, 182, 109, 1, 120, 164, 135, 173, 224, 114, 21, 89, 92, 226, 123, 34, 120, 166, 116, 209, 205, 149, 2, 215, 149, 209, 39, 127, 126, 128, 147, 35, 224, 250, 63, 41, 114, 92, 5, 31, 2, 46, 90, 95, 173, 99, 25, 134, 114, 172, 226, 223, 60, 227, 62, 116, 92, 42, 97, 169, 58, 9, 180, 25, 95, 215, 200, 56, 28, 53, 49, 8, 196, 147, 2, 251, 71, 75, 200, 244, 167, 180, 155, 4, 63, 84, 134, 25, 15, 152, 40, 58, 53];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SecureWithSalt()
        {
            var hash = Array.Empty<byte>();
            var salt = Array.Empty<byte>();
            var actual = hash.Secure(salt);

            Assert.Empty(actual);

            hash = [3, 4];
            actual = hash.Secure(salt);

            Assert.Equal(hash, actual);

            salt = [1, 2];
            actual = hash.Secure(salt);

            Assert.NotNull(actual);
            Assert.Equal(128, actual.Length);

            byte[] expected = [96, 108, 67, 93, 185, 93, 26, 167, 81, 244, 52, 72, 146, 17, 1, 216, 151, 244, 177, 27, 142, 47, 107, 95, 7, 91, 141, 102, 61, 153, 222, 15, 234, 46, 216, 242, 182, 109, 1, 120, 164, 135, 173, 224, 114, 21, 89, 92, 226, 123, 34, 120, 166, 116, 209, 205, 149, 2, 215, 149, 209, 39, 127, 126, 128, 147, 35, 224, 250, 63, 41, 114, 92, 5, 31, 2, 46, 90, 95, 173, 99, 25, 134, 114, 172, 226, 223, 60, 227, 62, 116, 92, 42, 97, 169, 58, 9, 180, 25, 95, 215, 200, 56, 28, 53, 49, 8, 196, 147, 2, 251, 71, 75, 200, 244, 167, 180, 155, 4, 63, 84, 134, 25, 15, 152, 40, 58, 53];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Veil()
        {
            var value = string.Empty;
            var actual = value.Veil([1, 2, 3]);

            Assert.Equal(string.Empty, actual);

            value = null;
            actual = value.Veil([1, 2, 3]);

            Assert.Null(actual);

            value = string.Empty;
            actual = value.Veil("secret");

            Assert.Equal(string.Empty, actual);

            value = null;
            actual = value.Veil("secret");

            Assert.Null(actual);
        }
    }
}
