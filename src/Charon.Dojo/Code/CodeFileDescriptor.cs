namespace Charon.Dojo.Code
{
    public sealed class CodeFileDescriptor
    {
        private readonly CodeFileArguments _arguments = new();

        public CodeFileDescriptor(out CodeFileArguments arguments)
        {
            arguments = _arguments;
        }

        public CodeFileDescriptor Using(string name)
        {
            _arguments.Usings ??= [];
            _arguments.Usings.Add(name);

            return this;
        }

        public CodeFileDescriptor Namespace(string name)
        {
            _arguments.NamespaceName = name;

            Using(name);

            return this;
        }
    }
}
