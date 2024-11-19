namespace Charon.Core.Tests
{
    public sealed class TypeNameExtensionsTests
    {
        [Fact]
        public void GetNameOfEnum()
        {
            Assert.Equal("System.ConsoleColor.Black", ConsoleColor.Black.GetName());
        }

        [Fact]
        public void GetNameOfType()
        {
            Assert.Equal("Charon.Core.Tests.TypeNameExtensionsTests", GetType().GetName());
        }
    }
}
