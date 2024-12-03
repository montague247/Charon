using Charon.Types;

namespace Charon.Core.Tests.Types
{
    public sealed class PriorityAttributeTests
    {
        [Fact]
        public void Init()
        {
            var actual = new PriorityAttribute(42);
            Assert.Equal(42, actual.Priority);
        }
    }
}
