namespace Charon.Dojo.Code
{
    public abstract class CommonCodeBuilder : IBuilder
    {
        private readonly CodeWriter _writer;
        private readonly CodeFileArguments _arguments;

        public CommonCodeBuilder(CodeWriter writer, CodeFileArguments arguments)
        {
            _writer = writer;
            _arguments = arguments;
        }

        public CommonCodeBuilder(CommonCodeBuilder commonCodeBuilder)
        {
            _writer = commonCodeBuilder._writer;
            _arguments = commonCodeBuilder._arguments;
        }

        public CodeFileArguments Arguments { get { return _arguments; } }

        public string Build() => _writer.Build();

        public abstract void Build(IndentedWriter writer);

        public TypeBlock GetName<T>()
        {
            return GetName(typeof(T));
        }

        public TypeBlock GetName(Type type)
        {
            _arguments.AddUsing(type.Namespace!);

            return new TypeBlock(_arguments.TypeName, type);
        }

        public EnumBlock GetName(Enum e)
        {
            _arguments.AddUsing(e.GetType().Namespace!);

            return new EnumBlock(_arguments.TypeName, e);
        }
    }
}
