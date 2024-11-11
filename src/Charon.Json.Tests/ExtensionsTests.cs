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

        [Fact]
        public void ToJsonFile()
        {
            var path = Path.GetFullPath($"{nameof(ExtensionsTests)}_{nameof(ToJsonFile)}.json");
            new Foo().ToJson(path);

            var content = File.ReadAllText(path);
            Assert.Equal("{\n  \"Hint\": null,\n  \"Bars\": null\n}\n", content);
        }
  
        [Fact]
        public void ToJsonFileCompact()
        {
            var path = Path.GetFullPath($"{nameof(ExtensionsTests)}_{nameof(ToJsonFile)}.json");
            new Foo().ToJson(path, true);

            var content = File.ReadAllText(path);
            Assert.Equal("{\"Hint\":null,\"Bars\":null}", content);
        }
  }
}
