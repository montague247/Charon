using Charon.Dojo;

namespace Charon.Core.Tests.Dojo
{
    public sealed class AssemblyNamespaceDojoAttributeTests
    {
        [Fact]
        public void Init()
        {
            var actual = new AssemblyNamespaceDojoAttribute("foo.bar");
            Assert.Equal("foo.bar", actual.Namespace);
            Assert.True(actual.JsonTests);
        }
    }
}
