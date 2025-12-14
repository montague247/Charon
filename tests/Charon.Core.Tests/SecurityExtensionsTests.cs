using System.Diagnostics;
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
            var secured = "fGdxZz90bjeBUE44dThZMVdaSk12ZzteOjFQWDxfTG9HTD5aUmpsfUhENX5xU3l1Tmt2bzI7O3I1SVo5ZmlxRzxTO3w7R1VSb0VIRVNJXUZoRTJmdDVKTy85UFpwdDg3NFhrV2NnRkteb2N0XD1XblBFNVJmW2gwU1Ncbml4e09WOkdIS1dmWVRcUlJZcVdcWHN5TGh4WHlsenxJQ2hNf3QzV0Vqenh1NXF1dTpre25VSlt2R1dyeFVTcWs7OVA2fWo9WTRmVm9TZGRzX2Rwc2s5OmtkZ0tQe0htclI6SHZYcTNGXUxlbDpadEw1SWc5NkVLNEZNXHB5VVVKbVtHe09ZUnhbeHJJdE9rezNbVmlmOnRITlRzdUNPZGZyaFtmV2lDXEZcbW1xZ1ZReE5QZ3I6R1RrVnVUTVFHOG01dVpadG45dlR5XHN2ZC08VzQ5Tnk1am5SSmxVOHBnc0pKaXdna0R6OTc4cmNpbzoxc3NKN0VVWn1yMkhJRXA0Nng9STE2SVRnbzRuRlRzOE9abUpQaTh0elJ7UjlTfDtcRk1LUnNqWHw8eD5aZTJHVnh0e3o6Umx2em5DZ3V9ak1Rb09HamdIaG51OjM9Z1RIcnI1eHxRT3xKRzJOUkNWakhseFFlcGdLZmZqPGh2TVJXbHVLOktRcUp2cjpLNTw9dS1VZ39lMXhxXGNZOVJ3NTg4UncyNFNtUHdPe2s2S0MuallvcThdTng2Tm9HcGo4b1VRXDZMcWNtRUxublVFW3dLezZpMHVyVkVGdG9LM1tMR1tNfVBrO107dFY5VWhbN1hyVWNHZTdRczJsTV5nWDw7aVl1VXl9Q1RcVXFWMUh3NXk0NFtscHx7aElsQ2Q5NTVPUm9RSXlSZVxxUUU9am84UnY7MnZSZ39CXExuUTEySlBvezJA";
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
            var secured = "QVMMlgnP00keG7tA2yeeurPljFb6illQog41cRUbDtk4WXbPROHfQkNO2olPQ0wfGjWVkWomXdAC5rT8sFkDhhZozoz+ophA2T9+TrdcatXQ6lIiM4FZdIArXG4cDETgR4yCqD0g7/djJtNtclhbDAc/operKD7kA9rgHXBDzZ07PwCKAtPZZtA+n03UT5bi3CJFQA1ua4JaLx1UFDlD7VdJZuUs9dsaRQncS0JgKzIZD8TSsOJayRgL3PqhfLWB9ToIqSamrEAKQbQwwEFluLoDbEcGZURrpBUydP8ODetAi27JbBUAH2VI/lBRhDsAOh67w54WVM0ZAa0mr1TCYDFDGIVI1c6NEjWQPtdBYvjIAI0lHYsxVpzwL0gGLkQCWXtoqyvj9yI/HwCCNFE8Mxn1tqeeJ3TG2MTL+KxD3Yc6PPaeA6WpjOdCczPGY3LVtCldHSVvLWGAI1tU+BUQ8SSMcM5IKP0BZS4CbS9YMT4pFbXDmBd7wxAP8/dvPPqqMmPfhRHUtGbyJ5QttG6T8+rqZCETfm6KfzcyTNoHTc5ASVGuZxnPFlUf/oo0PUkiBQi9o34fcsjv7dwYb3bTaP0wzmIozJdm6gqzZOomqgHN5VAeAntNgZX1IVL8Ni8Bb2GT314T9h5wMeCKTF4rEQEMhLhu7ILyGRLKCp1MuXgfHgWh+pWfjPIynm3GanPe2eaSDwhcSWJdGTUz7z804Wl5WMxcJPQhcEP+TkpXGDQjDnarbgE61RH/0ACfdrmJJ0bhfgaNun/qK5tmvkl5+NgWhj4iTDhXWzk3ceguKtk5XJXSR/Oz+U4u/4lscEw+Sip9t2v2g636E/T4fGLbdQ9fGIohydyABzi+R+VlqMvsI2AkKmF7doElG03VCknxJFxnxWMk1CFhRwWGXFtU+CYKmcZ0";
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

            byte[] expected = [247, 236, 204, 192, 23, 79, 10, 228, 100, 60, 82, 43, 70, 124, 84, 240, 209, 130, 207, 40, 72, 42, 253, 140, 206, 38, 56, 130, 183, 120, 107, 144, 217, 1, 88, 203, 151, 74, 103, 225, 181, 255, 148, 150, 29, 27, 92, 246, 162, 186, 162, 231, 33, 25, 223, 50, 254, 65, 44, 95, 191, 206, 121, 169, 185, 163, 223, 210, 165, 25, 174, 47, 135, 38, 66, 199, 153, 201, 133, 40, 17, 11, 111, 73, 220, 79, 231, 87, 86, 222, 180, 171, 2, 41, 210, 247, 147, 160, 203, 115, 52, 209, 19, 250, 138, 149, 219, 58, 171, 81, 92, 75, 117, 34, 219, 148, 166, 136, 232, 249, 85, 221, 111, 1, 50, 237, 6, 232];
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

            byte[] expected = [247, 236, 204, 192, 23, 79, 10, 228, 100, 60, 82, 43, 70, 124, 84, 240, 209, 130, 207, 40, 72, 42, 253, 140, 206, 38, 56, 130, 183, 120, 107, 144, 217, 1, 88, 203, 151, 74, 103, 225, 181, 255, 148, 150, 29, 27, 92, 246, 162, 186, 162, 231, 33, 25, 223, 50, 254, 65, 44, 95, 191, 206, 121, 169, 185, 163, 223, 210, 165, 25, 174, 47, 135, 38, 66, 199, 153, 201, 133, 40, 17, 11, 111, 73, 220, 79, 231, 87, 86, 222, 180, 171, 2, 41, 210, 247, 147, 160, 203, 115, 52, 209, 19, 250, 138, 149, 219, 58, 171, 81, 92, 75, 117, 34, 219, 148, 166, 136, 232, 249, 85, 221, 111, 1, 50, 237, 6, 232];
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
