using Charon.Types.Update;

namespace Charon.Core.Tests.Types.Update
{
    public sealed class SourceValueAttributeTests
    {
        [Fact]
        public void Init()
        {
            var attr = new SourceValueAttribute("Hello");
            Assert.Equal("Hello", attr.Value);

            attr = new SourceValueAttribute(42);
            Assert.Equal(42, attr.Value);
        }
    }
}