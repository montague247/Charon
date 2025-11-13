using System.Security.Cryptography;

namespace Charon.Security;

public sealed class SecurityLevel4 : SecurityLevel
{
    public SecurityLevel4() : base(4)
    {
    }

    protected override int KeySize => 2048;

    protected override int DeriveBytesIterations => 50000;

    protected override HashAlgorithmName DeriveBytesHashAlgorithmName => HashAlgorithmName.SHA512;

    protected override int DerivedBytesLength => 128;
}
