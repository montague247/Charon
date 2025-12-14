using System.Diagnostics;
using System.Net.WebSockets;
using Charon.Encoder.QR;

namespace Charon.Core.Tests.Encoder.QR;

public sealed class GeneratorTests
{
    [Fact]
    public void GenerateLow()
    {
        const string text = "Hello, World!";
        var actual = Generator.Generate(text, EccLevel.Low, out int size);
        Assert.NotNull(actual);
        Assert.Equal(21, size);
        var array = actual.ToStringArray(size);
        Assert.NotNull(array);

        var idx = 0;
        Assert.Equal("111111100111001111111", array[idx++]);
        Assert.Equal("100000101010001000001", array[idx++]);
        Assert.Equal("101110100111001011101", array[idx++]);
        Assert.Equal("101110101000001011101", array[idx++]);
        Assert.Equal("101110100000001011101", array[idx++]);
        Assert.Equal("100000101101101000001", array[idx++]);
        Assert.Equal("111111101010101111111", array[idx++]);
        Assert.Equal("000000000000100000000", array[idx++]);
        Assert.Equal("111110111101101011111", array[idx++]);
        Assert.Equal("110101011110010101001", array[idx++]);
        Assert.Equal("100110111100011101010", array[idx++]);
        Assert.Equal("000101011001010001000", array[idx++]);
        Assert.Equal("100101100100001000101", array[idx++]);
        Assert.Equal("000000001000111011100", array[idx++]);
        Assert.Equal("111111100100011000010", array[idx++]);
        Assert.Equal("100000101000010001010", array[idx++]);
        Assert.Equal("101110100011011110111", array[idx++]);
        Assert.Equal("101110101100100011100", array[idx++]);
        Assert.Equal("101110100101001000000", array[idx++]);
        Assert.Equal("100000101110010111000", array[idx++]);

        actual = Generator.Generate(text, EccLevel.Medium, out size);
        Assert.NotNull(actual);
        Assert.Equal(21, size);
        array = actual.ToStringArray(size);
        Assert.NotNull(array);
        WriteAsserts(array);

        idx = 0;
        Assert.Equal("111111101101101111111", array[idx++]);
        Assert.Equal("100000100101001000001", array[idx++]);
        Assert.Equal("101110100111001011101", array[idx++]);
        Assert.Equal("101110101001101011101", array[idx++]);
        Assert.Equal("101110101110001011101", array[idx++]);
        Assert.Equal("100000101111101000001", array[idx++]);
        Assert.Equal("111111101010101111111", array[idx++]);
        Assert.Equal("000000001100100000000", array[idx++]);
        Assert.Equal("100010111111101010001", array[idx++]);
        Assert.Equal("101111011000010101001", array[idx++]);
        Assert.Equal("111011101110011101010", array[idx++]);
        Assert.Equal("111010011101010001000", array[idx++]);
        Assert.Equal("001001100110001000101", array[idx++]);
        Assert.Equal("000000001010111011100", array[idx++]);
        Assert.Equal("111111101100011000010", array[idx++]);
        Assert.Equal("100000101010010001010", array[idx++]);
        Assert.Equal("101110101111011110111", array[idx++]);
        Assert.Equal("101110101010100011100", array[idx++]);
        Assert.Equal("101110100101001000000", array[idx++]);
        Assert.Equal("100000100010010111000", array[idx++]);
    }

    [Fact]
    public void GenerateAlphaNum()
    {
        const string text = "Hello World.";
        var actual = Generator.Generate(text, EccLevel.Low, out int size);
        Assert.NotNull(actual);
        Assert.Equal(21, size);
        var array = actual.ToStringArray(size);
        Assert.NotNull(array);

        var idx = 0;
        Assert.Equal("111111101111001111111", array[idx++]);
        Assert.Equal("100000101010001000001", array[idx++]);
        Assert.Equal("101110101111001011101", array[idx++]);
        Assert.Equal("101110101000001011101", array[idx++]);
        Assert.Equal("101110100000001011101", array[idx++]);
        Assert.Equal("100000101101101000001", array[idx++]);
        Assert.Equal("111111101010101111111", array[idx++]);
        Assert.Equal("000000000000100000000", array[idx++]);
        Assert.Equal("110011100101100110011", array[idx++]);
        Assert.Equal("001100000001011001011", array[idx++]);
        Assert.Equal("010011111010001101101", array[idx++]);
        Assert.Equal("100101000111010000100", array[idx++]);
        Assert.Equal("100000110110001100000", array[idx++]);
        Assert.Equal("000000000000000101111", array[idx++]);
        Assert.Equal("111111100101100111000", array[idx++]);
        Assert.Equal("100000101010100000001", array[idx++]);
        Assert.Equal("101110100101100101010", array[idx++]);
        Assert.Equal("101110101111011001000", array[idx++]);
        Assert.Equal("101110101010001111100", array[idx++]);
        Assert.Equal("100000101101011100001", array[idx++]);

        actual = Generator.Generate(text, EccLevel.Medium, out size);
        Assert.NotNull(actual);
        Assert.Equal(21, size);
        array = actual.ToStringArray(size);
        Assert.NotNull(array);
        WriteAsserts(array);

        idx = 0;
        Assert.Equal("111111101001001111111", array[idx++]);
        Assert.Equal("100000101010101000001", array[idx++]);
        Assert.Equal("101110101110001011101", array[idx++]);
        Assert.Equal("101110100111101011101", array[idx++]);
        Assert.Equal("101110101110001011101", array[idx++]);
        Assert.Equal("100000100111101000001", array[idx++]);
        Assert.Equal("111111101010101111111", array[idx++]);
        Assert.Equal("000000000000100000000", array[idx++]);
        Assert.Equal("100111111001101111001", array[idx++]);
        Assert.Equal("110010001111011001011", array[idx++]);
        Assert.Equal("011110101010001101101", array[idx++]);
        Assert.Equal("000000001101010000100", array[idx++]);
        Assert.Equal("111000100100001100000", array[idx++]);
        Assert.Equal("000000001010000101111", array[idx++]);
        Assert.Equal("111111100011100111000", array[idx++]);
        Assert.Equal("100000100010100000001", array[idx++]);
        Assert.Equal("101110101111100101010", array[idx++]);
        Assert.Equal("101110100111011001000", array[idx++]);
        Assert.Equal("101110101000001111100", array[idx++]);
        Assert.Equal("100000101001011100001", array[idx++]);
    }

    private static void WriteAsserts(string[] array)
    {
        foreach (var line in array)
        {
            Debug.WriteLine($"Assert.Equal(\"{line}\", array[idx++]);");
        }
    }
}
