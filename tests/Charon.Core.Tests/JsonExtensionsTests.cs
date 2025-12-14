using Charon.Core.Tests.Types;
using Xunit;

namespace Charon.Core.Tests
{
    public sealed class JsonExtensionsTests
    {
        [Fact]
        public void ToJson()
        {
            var actual = new Foo().ToJson();

            Assert.Equal("{\n  \"hint\": null,\n  \"bars\": null\n}", actual);
        }

        [Fact]
        public void ToJsonCompact()
        {
            var actual = new Foo().ToJson(true);

            Assert.Equal("{\"hint\":null,\"bars\":null}", actual);
        }

        [Fact]
        public void NullToJson()
        {
            object? obj = null;
            var actual = obj.ToJson();

            Assert.Equal("null", actual);
        }

        [Fact]
        public void ToJsonFile()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(ToJsonFile)}.json");
            new Foo().ToJson(path);

            var content = File.ReadAllText(path);
            Assert.Equal("{\n  \"hint\": null,\n  \"bars\": null\n}\n", content);
        }

        [Fact]
        public void ToJsonFileCompact()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(ToJsonFile)}.json");
            new Foo().ToJson(path, true);

            var content = File.ReadAllText(path);
            Assert.Equal("{\"hint\":null,\"bars\":null}", content);
        }

        [Fact]
        public void NullToJsonFile()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(NullToJsonFile)}.json");
            object? obj = null;
            obj.ToJson(path);

            Assert.False(File.Exists(path));
        }

        [Fact]
        public void FromFileExisting()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(FromFileExisting)}.json");
            new Foo().ToJson(path);

            var acutal = JsonExtensions.FromFile<Foo>(path);
            Assert.NotNull(acutal);
        }

        [Fact]
        public void FromFileNotExistingCreate()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(FromFileNotExistingCreate)}.json");
            var acutal = JsonExtensions.FromFile<Foo>(path);
            Assert.NotNull(acutal);
        }

        [Fact]
        public void FromFileNotExistingNoCreate()
        {
            var path = Path.GetFullPath($"{GetType().Name}_{nameof(FromFileNotExistingNoCreate)}.json");
            var acutal = JsonExtensions.FromFile<Foo>(path, false);
            Assert.Null(acutal);
        }
    }
}
