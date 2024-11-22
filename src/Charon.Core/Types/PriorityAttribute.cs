namespace Charon.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PriorityAttribute(int priority) : Attribute
    {
        public int Priority { get; } = priority;
    }
}
