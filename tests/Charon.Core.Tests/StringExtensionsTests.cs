namespace Charon.Core.Tests
{
    public sealed class StringExtensionsTests
    {
        [Fact]
        public void SingleLineFromNull()
        {
            string? expected = null;
            var actual = expected.SingleLine();

            Assert.Equal(expected, actual);
            Assert.Null(actual);
        }

        [Fact]
        public void SingleLineFromEmpty()
        {
            var expected = string.Empty;
            var actual = expected.SingleLine()!;

            Assert.Equal(expected, actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void SingleLineSimple()
        {
            var expected = "Hallo";
            var actual = expected.SingleLine()!;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleLine()
        {
            var expected = "a\\nb\\rc\\td\\\\e\\0f\"g";
            var actual = "a\nb\rc\td\\e\0f\"g".SingleLine();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HasCharsToEscape()
        {
            string? nullString = null;

            Assert.False(nullString.HasCharsToEscape());
            Assert.False(string.Empty.HasCharsToEscape());
            Assert.False("Hallo".HasCharsToEscape());
            Assert.True("\n".HasCharsToEscape());
            Assert.True("\r".HasCharsToEscape());
            Assert.True("\t".HasCharsToEscape());
            Assert.True("\\".HasCharsToEscape());
            Assert.True("\"".HasCharsToEscape());
            Assert.True("\0".HasCharsToEscape());
        }

        [Fact]
        public void ToEscapedStringSingleLine()
        {
            string? nullString = null;

            Assert.Null(nullString.ToEscapedString());
            Assert.Equal("\"foo\\tbar\"", "foo\tbar".ToEscapedString());
        }

        [Fact]
        public void ToEscapedStringMultiLine()
        {
            Assert.Equal("@\"foo\tbar\"", "foo\tbar".ToEscapedString(false));
            Assert.Equal("\"Hello world\"", "Hello world".ToEscapedString(false));
        }

        [Fact]
        public void CreateGuid()
        {
            Assert.Equal(new Guid("712677e6-b57e-c073-ed58-9bb1f967c689"), "Dirk rockz".CreateGuid());
        }
    }
}
