using Charon.Types;

namespace Charon.Core.Tests.Types
{
    public sealed class LocalEnvironmentTests
    {
        [Fact]
        public void Instance()
        {
            var actual = LocalEnvironment.Instance;
            Assert.NotNull(actual);
            Assert.Equal("DEV", actual.Stage);
            Assert.Equal("Development", actual.Area);
            Assert.Equal("Local", actual.Zone);
            Assert.Equal("DEV-Development-Local", actual.FullName);
        }

        [Fact]
        public void Object()
        {
            var actual = new LocalEnvironment
            {
                Stage = "TESTS",
                Area = "Dev",
                Zone = "Lcl"
            };
            Assert.NotNull(actual);
        }
    }
}