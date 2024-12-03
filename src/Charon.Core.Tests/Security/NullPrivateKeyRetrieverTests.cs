using Charon.Security;

namespace Charon.Core.Tests.Security
{
    public sealed class NullPrivateKeyRetrieverTests
    {
        [Fact]
        public void Init()
        {
            var retriever = new NullPrivateKeyRetriever();
            Assert.Null(retriever.GetHash());
            Assert.Null(retriever.GetKey());
        }
    }
}
