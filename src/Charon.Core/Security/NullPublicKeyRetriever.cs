using Charon.Types;

namespace Charon.Security
{
    [Priority(int.MinValue)]
    public sealed class NullPublicKeyRetriever : IPublicKeyRetriever
    {
        public string DefaultStage => LocalEnvironment.Instance.Stage;

        public int KeySize => -1;

        public byte[]? GetKey(string stage) => null;
    }
}
