using Xunit;

namespace Charon.Json.Tests
{
    public sealed class ExtensionsTests
    {
        [Fact]
        public void ToJson()
        {
            var actual = new Foo().ToJson();

            Assert.Equal("{\"hint\":null,\"bars\":null}", actual);
        }
    }
}
