namespace Charon
{
    public static class TypeNameExtensions
    {
        public static string GetName(this Type type) => TypeName.Instance.GetName(type)!;

        public static string GetName(this Enum e) => TypeName.Instance.GetName(e)!;
    }
}
