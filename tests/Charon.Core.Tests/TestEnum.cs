using System.ComponentModel;
using System.Runtime.Serialization;

namespace Charon.Core.Tests
{
    public enum TestEnum
    {
        EnumName,
        [EnumMember(Value = "Enum Member Value")]
        EnumMemberValue,
        [Description("from description")]
        Description,
        [EnumMember(Value = "Enum Member Value with description")]
        [Description("Enum Member Value description only")]
        EnumMemberValueWithDescription
    }
}
