namespace Charon.Dojo.Code
{
    public sealed class MethodDescriptor
    {
        private readonly MethodArguments _arguments;
        private readonly CodeFileArguments _codeFileArguments;

        public MethodDescriptor(string name, CodeFileArguments codeFileArguments, out MethodArguments arguments)
        {
            _arguments = arguments = new(name);
            _codeFileArguments = codeFileArguments;
        }

        public MethodDescriptor Accessibility(Accessibility accessibility)
        {
            _arguments.Accessibility = accessibility;

            return this;
        }

        public MethodDescriptor ReturnType<T>()
        {
            return ReturnType(typeof(T));
        }

        public MethodDescriptor ReturnType(Type type)
        {
            if (!TypeName.IsSimple(type))
                _codeFileArguments.AddUsing(type.Namespace!);

            _arguments.ReturnType = new TypeBlock(_codeFileArguments.TypeName, type);

            return this;
        }

        public MethodDescriptor Argument<T>(string name)
        {
            return Argument(name, typeof(T));
        }

        public MethodDescriptor Argument(string name, Type type)
        {
            if (!TypeName.IsSimple(type))
                _codeFileArguments.AddUsing(type.Namespace!);

            _arguments.Arguments ??= [];
            _arguments.Arguments.Add(new(name) { Type = new TypeBlock(_codeFileArguments.TypeName, type) });

            return this;
        }
    }
}
