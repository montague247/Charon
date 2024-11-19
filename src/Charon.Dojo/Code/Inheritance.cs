using System.Runtime.Serialization;

namespace Charon.Dojo.Code
{
    public enum Inheritance
    {
        Default = 0,
        [EnumMember(Value = "abstract")]
        Abstract = 1,
        [EnumMember(Value = "sealed")]
        Sealed = 2,
        [EnumMember(Value = "static")]
        Static = 3
    }
}
