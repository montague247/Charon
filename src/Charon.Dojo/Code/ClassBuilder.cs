using System.Text;

namespace Charon.Dojo.Code
{
    public sealed class ClassBuilder(CodeWriter writer, CodeFileArguments codeFileArguments, ClassArguments arguments) : CommonCodeBuilder(writer, codeFileArguments)
    {
        private readonly ClassArguments _arguments = arguments;
        private bool _empty;
        private List<MethodBuilder>? _methodBuilders;

        public ClassBuilder Method(string name, Action<MethodDescriptor> descriptor, Action<CodeBuilder> code)
        {
            descriptor(new MethodDescriptor(name, Arguments, out MethodArguments methodsArguments));

            var codeBuilder = new CodeBuilder(this);
            code(codeBuilder);

            _methodBuilders ??= [];
            _methodBuilders.Add(new MethodBuilder(this, methodsArguments, codeBuilder));

            return this;
        }

        public override void Build(IndentedWriter writer)
        {
            writer.WriteLine(BuildClass());
            writer.WriteLine("{");
            writer.Indent();

            _empty = true;

            BuildMethods(writer);

            writer.Unindent();
            writer.WriteLine("}");
        }

        private void BuildMethods(IndentedWriter writer)
        {
            if (_methodBuilders == null ||
                _methodBuilders.Count == 0)
                return;

            foreach (var builder in _methodBuilders)
            {
                if (_empty)
                    _empty = false;
                else
                    writer.NewLine();

                builder.Build(writer);
            }
        }

        private string BuildClass()
        {
            var sb = new StringBuilder(_arguments.Accessibility.Value());

            if (_arguments.Inheritance != Inheritance.Default)
                sb.Append(' ').Append(_arguments.Inheritance.Value());

            if (_arguments.Partial)
                sb.Append(" partial");

            sb.Append(" class ").Append(_arguments.Name);

            if (_arguments.Inherits != null)
                BuildInherit(sb);

            return sb.ToString();
        }

        private void BuildInherit(StringBuilder sb)
        {
            var first = true;

            foreach (var inherit in _arguments.Inherits!)
            {
                if (first)
                {
                    first = false;
                    sb.Append(" : ");
                }
                else
                    sb.Append(", ");

                sb.Append(inherit.ToString());
            }
        }
    }
}
