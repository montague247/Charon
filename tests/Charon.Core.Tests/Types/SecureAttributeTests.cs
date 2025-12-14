using Charon.Types;

namespace Charon.Core.Tests.Types
{
    public sealed class SecureAttributeTests
    {
        [Fact]
        public void Init()
        {
            var attr = new SecureAttribute();
            Assert.True(attr.CreateExtensions);

            attr = new SecureAttribute(false);
            Assert.False(attr.CreateExtensions);
        }
    }
}
