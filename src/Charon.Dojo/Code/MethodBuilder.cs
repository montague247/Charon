using System.Text;

namespace Charon.Dojo.Code
{
    public sealed class MethodBuilder(CommonCodeBuilder commonCodeBuilder, MethodArguments arguments, CodeBuilder codeBuilder) : CommonCodeBuilder(commonCodeBuilder)
    {
        private readonly MethodArguments _arguments = arguments;
        private readonly CodeBuilder _codeBuilder = codeBuilder;

        public override void Build(IndentedWriter writer)
        {
            writer.WriteLine(BuildMethod());
            writer.WriteLine("{");
            writer.Indent();

            _codeBuilder.Build(writer);

            writer.Unindent();
            writer.WriteLine("}");
        }

        private string BuildMethod()
        {
            var sb = new StringBuilder(_arguments.Accessibility.Value());

            if (_arguments.ReturnType == null)
                sb.Append(" void");
            else
                sb.Append(' ').Append(_arguments.ReturnType.ToString());

            sb.Append(' ').Append(_arguments.Name);
            sb.Append('(');

            if (_arguments.Arguments != null)
            {
                var first = true;

                foreach (var argument in _arguments.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(argument.Type).Append(' ').Append(argument.Name);
                }
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
