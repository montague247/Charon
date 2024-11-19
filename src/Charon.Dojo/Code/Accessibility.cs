using System.Runtime.Serialization;

namespace Charon.Dojo.Code
{
    public enum Accessibility
    {
        [EnumMember(Value = "public")]
        Public = 0,
        [EnumMember(Value = "protected")]
        Protected = 1,
        [EnumMember(Value = "internal")]
        Internal = 2,
        [EnumMember(Value = "private")]
        Private = 3
    }
}
