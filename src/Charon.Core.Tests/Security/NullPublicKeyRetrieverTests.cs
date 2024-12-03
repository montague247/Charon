using Charon.Security;

namespace Charon.Core.Tests.Security
{
    public sealed class NullPublicKeyRetrieverTests
    {
        [Fact]
        public void Init()
        {
            var retriever = new NullPublicKeyRetriever();
            Assert.Equal("DEV", retriever.DefaultStage);
            Assert.Equal(-1, retriever.KeySize);
            Assert.Null(retriever.GetKey("any"));
            Assert.Null(retriever.GetKey("DEV"));
        }
    }
}
