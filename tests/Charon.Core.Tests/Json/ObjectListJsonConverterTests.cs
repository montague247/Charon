using System.Text.Json;
using Charon.Core.Tests.Types;

namespace Charon.Core.Tests.Json;

public sealed class ObjectListJsonConverterTests
{
    [Fact]
    public void WriteNull()
    {
        var sourceObject = new Foo
        {
            Hint = "test hint"
        };

        var json = JsonSerializer.Serialize(sourceObject, sourceObject.GetType());
        Assert.Equal("{\"Hint\":\"test hint\",\"Bars\":null}", json);
    }

    [Fact]
    public void ReadNull()
    {
        var json = "{\"Hint\":\"test hint\",\"Bars\":null}";
        var actual = JsonSerializer.Deserialize<Foo>(json);

        Assert.NotNull(actual);
        Assert.Null(actual.Bars);
    }

    [Fact]
    public void WriteSingle()
    {
        var sourceObject = new Foo
        {
            Hint = "test hint",
            Bars =
            [
                new()
            ]
        };

        var json = JsonSerializer.Serialize(sourceObject, sourceObject.GetType());
        Assert.Equal("{\"Hint\":\"test hint\",\"Bars\":{\"Enabled\":true,\"Message\":null}}", json);
    }

    [Fact]
    public void ReadSingle()
    {
        var json = "{\"Hint\":\"test hint\",\"Bars\":{\"Enabled\":true,\"Message\":null}}";
        var actual = JsonSerializer.Deserialize<Foo>(json);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Bars);
        Assert.Single(actual.Bars);
    }

    [Fact]
    public void WriteMultiple()
    {
        var sourceObject = new Foo
        {
            Hint = "test hint",
            Bars =
            [
                new(),
                new()
            ]
        };

        var json = JsonSerializer.Serialize(sourceObject, sourceObject.GetType());
        Assert.Equal("{\"Hint\":\"test hint\",\"Bars\":[{\"Enabled\":true,\"Message\":null},{\"Enabled\":true,\"Message\":null}]}", json);
    }

    [Fact]
    public void ReadMultiple()
    {
        var json = "{\"Hint\":\"test hint\",\"Bars\":[{\"Enabled\":true,\"Message\":null},{\"Enabled\":true,\"Message\":null}]}";
        var actual = JsonSerializer.Deserialize<Foo>(json);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Bars);
        Assert.Equal(2, actual.Bars.Count);
    }
}
