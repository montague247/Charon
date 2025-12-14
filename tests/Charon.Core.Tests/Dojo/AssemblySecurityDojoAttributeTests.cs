using Charon.Dojo;

namespace Charon.Core.Tests.Dojo
{
    public sealed class AssemblySecurityDojoAttributeTests
    {
        [Fact]
        public void Init()
        {
            var actual = new AssemblySecurityDojoAttribute("SE-Name");

            Assert.Equal("SE-Name", actual.Name);
            Assert.Null(actual.Path);
            Assert.Null(actual.Namespace);
            Assert.EndsWith(Path.Combine("src", "Charon.Core.Tests", "Dojo"), actual.SourcePath);
            Assert.Equal(["DEV"], actual.Stages);
            Assert.Null(actual.ConfigurationPath);
            Assert.Equal(4096, actual.KeySize);
            Assert.Equal(int.MaxValue, actual.Priority);

            actual.EnsureNamespace(GetType().Assembly);

            Assert.Equal("SE-Name", actual.Name);
            Assert.Null(actual.Path);
            Assert.Equal("Charon.Core.Tests", actual.Namespace);
            Assert.EndsWith(Path.Combine("src", "Charon.Core.Tests", "Dojo"), actual.SourcePath);
            Assert.Equal(["DEV"], actual.Stages);
            Assert.Null(actual.ConfigurationPath);
            Assert.Equal(4096, actual.KeySize);
            Assert.Equal(int.MaxValue, actual.Priority);
        }

        [Fact]
        public void InitFull()
        {
            var actual = new AssemblySecurityDojoAttribute("SE-Name-full", "Subname", "myNamespace")
            {
                Stages = ["DEV", "QA", "PROD"],
                ConfigurationPath = "mySecureEncrypt.json",
                KeySize = 2048,
                Priority = 42
            };
            actual.EnsureNamespace(GetType().Assembly);

            Assert.Equal("SE-Name-full", actual.Name);
            Assert.Equal("Subname", actual.Path);
            Assert.Equal("myNamespace", actual.Namespace);
            Assert.EndsWith(Path.Combine("src", "Charon.Core.Tests", "Dojo"), actual.SourcePath);
            Assert.Equal(["DEV", "QA", "PROD"], actual.Stages);
            Assert.Equal("mySecureEncrypt.json", actual.ConfigurationPath);
            Assert.Equal(2048, actual.KeySize);
            Assert.Equal(42, actual.Priority);
        }

        [Fact]
        public void InitFull2()
        {
            var actual = new AssemblySecurityDojoAttribute("SE-Name-full", Path.Combine("Foo", "Bar"))
            {
                Stages = ["DEV", "QA", "PROD"],
                ConfigurationPath = "mySecureEncrypt.json",
                KeySize = 2048,
                Priority = 42
            };
            actual.EnsureNamespace(GetType().Assembly);

            Assert.Equal("SE-Name-full", actual.Name);
            Assert.Equal($"Foo{Path.DirectorySeparatorChar}Bar", actual.Path);
            Assert.Equal("Charon.Core.Tests.Foo.Bar", actual.Namespace);
            Assert.EndsWith(Path.Combine("src", "Charon.Core.Tests", "Dojo"), actual.SourcePath);
            Assert.Equal(["DEV", "QA", "PROD"], actual.Stages);
            Assert.Equal("mySecureEncrypt.json", actual.ConfigurationPath);
            Assert.Equal(2048, actual.KeySize);
            Assert.Equal(42, actual.Priority);
        }
    }
}
