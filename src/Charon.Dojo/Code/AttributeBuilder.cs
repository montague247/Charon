using System.Text;

namespace Charon.Dojo.Code
{
    public sealed class AttributeBuilder(CommonCodeBuilder commonCodeBuilder, AttributeArguments arguments) : CommonCodeBuilder(commonCodeBuilder)
    {
        private readonly AttributeArguments _arguments = arguments;

        public override void Build(IndentedWriter writer)
        {
            writer.WriteLine(BuildAttribute());
        }

        private string BuildAttribute()
        {
            var typeName = _arguments.Type!.ToString()[..^9];
            var sb = new StringBuilder("[").Append(typeName);

            if (_arguments.Arguments != null &&
                _arguments.Arguments.Count > 0)
            {
                sb.Append('(');
                var first = true;

                foreach (var argument in _arguments.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    if (argument.Name != null)
                    {
                        sb.Append(argument.Name);

                        if (char.IsUpper(argument.Name[0]))
                            sb.Append(" = ");
                        else
                            sb.Append(": ");
                    }

                    sb.Append(argument.Value);
                }

                sb.Append(')');
            }

            return sb.Append(']').ToString();
        }
    }
}
