namespace Charon.Types
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class SecureAttribute(bool createExtensions = true) : Attribute
    {
        public bool CreateExtensions { get; } = createExtensions;
    }
}
