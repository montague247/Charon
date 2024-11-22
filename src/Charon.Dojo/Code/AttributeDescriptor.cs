namespace Charon.Dojo.Code
{
    public sealed class AttributeDescriptor
    {
        private readonly AttributeArguments _arguments;
        private readonly CodeFileArguments _codeFileArguments;

        public AttributeDescriptor(CodeFileArguments codeFileArguments, out AttributeArguments arguments)
        {
            _arguments = arguments = new();
            _codeFileArguments = codeFileArguments;
        }

        public AttributeDescriptor Type<T>()
            where T : Attribute
        {
            return Type(typeof(T));
        }

        public AttributeDescriptor Type(Type type)
        {
            if (!TypeName.IsSimple(type))
                _codeFileArguments.AddUsing(type.Namespace!);

            _arguments.Type = new TypeBlock(_codeFileArguments.TypeName, type);

            return this;
        }

        public AttributeDescriptor Argument(object value)
        {
            return AddArgument(null, value);
        }

        public AttributeDescriptor Argument(string name, object? value = null)
        {
            return AddArgument(name, value);
        }

        private AttributeDescriptor AddArgument(string? name, object? value)
        {
            _arguments.Arguments ??= [];
            _arguments.Arguments.Add(new(name, value));

            return this;
        }
    }
}
