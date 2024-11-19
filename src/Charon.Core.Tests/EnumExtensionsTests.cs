namespace Charon.Core.Tests
{
    public sealed class EnumExtensionsTests
    {
        [Fact]
        public void NoAttributes()
        {
            Assert.Equal("EnumName", TestEnum.EnumName.ToString());
            Assert.Equal("EnumMemberValue", TestEnum.EnumMemberValue.ToString());
            Assert.Equal("Description", TestEnum.Description.ToString());
            Assert.Equal("EnumMemberValueWithDescription", TestEnum.EnumMemberValueWithDescription.ToString());
        }

        [Fact]
        public void Value()
        {
            Assert.Equal("EnumName", TestEnum.EnumName.Value());
            Assert.Equal("Enum Member Value", TestEnum.EnumMemberValue.Value());
            Assert.Equal("Description", TestEnum.Description.Value());
            Assert.Equal("Enum Member Value with description", TestEnum.EnumMemberValueWithDescription.Value());
        }

        [Fact]
        public void Description()
        {
            Assert.Equal("EnumName", TestEnum.EnumName.Description());
            Assert.Equal("Enum Member Value", TestEnum.EnumMemberValue.Description());
            Assert.Equal("from description", TestEnum.Description.Description());
            Assert.Equal("Enum Member Value description only", TestEnum.EnumMemberValueWithDescription.Description());
        }
    }
}
