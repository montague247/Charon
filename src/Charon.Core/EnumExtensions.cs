using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Charon
{
    public static class EnumExtensions
    {
        public static string? Value<T>(this T source)
           where T : Enum
        {
            var name = source.ToString();
            var attr = source.GetType().GetField(name)!.GetCustomAttribute<EnumMemberAttribute>(true);

            if (attr != null)
                return attr.Value;

            return name;
        }

        public static string? Description<T>(this T source)
            where T : Enum
        {
            var name = source.ToString();
            var attr = source.GetType().GetField(name)!.GetCustomAttribute<DescriptionAttribute>(true);

            if (attr != null)
                return attr.Description;

            return Value(source);
        }
    }
}
