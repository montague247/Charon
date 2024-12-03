using Charon.Types;

namespace Charon.Security
{
    [Priority(int.MinValue)]
    public sealed class NullPrivateKeyRetriever : IPrivateKeyRetriever
    {
        public byte[]? GetHash() => null;

        public byte[]? GetKey() => null;
    }
}
