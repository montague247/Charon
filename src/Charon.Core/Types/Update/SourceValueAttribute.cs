namespace Charon.Types.Update
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class SourceValueAttribute(object value) : Attribute
    {
        public object Value { get; } = value;
    }
}
