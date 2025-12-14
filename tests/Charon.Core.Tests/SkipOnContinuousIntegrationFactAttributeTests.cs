namespace Charon.Core.Tests
{
    public sealed class SkipOnContinuousIntegrationFactAttributeTests
    {
        [Fact]
        public void Init()
        {
            var fact = new SkipOnContinuousIntegrationFactAttribute();

#if DEBUG
            if (!global::System.Diagnostics.Debugger.IsAttached)
                Assert.Equal("No debugger attached", fact.Skip);
#else
                Assert.Equal("Skip on CI pipeline", fact.Skip);
#endif
        }
    }
}