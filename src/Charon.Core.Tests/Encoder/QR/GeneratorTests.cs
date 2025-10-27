using Charon.Encoder.QR;

namespace Charon.Core.Tests.Encoder.QR;

public sealed class GeneratorTests
{
    [Fact]
    public void Generate()
    {
        var actual = Generator.Generate("Hello, World!", EccLevel.Low, out int size);
        Assert.NotNull(actual);
        Assert.Equal(1, size);
        Assert.Equal("", actual.ToString(size));
    }
}
