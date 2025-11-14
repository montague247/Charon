using System.Text;

namespace Charon.Dojo.Code;

public sealed class PropertyBuilder(CommonCodeBuilder commonCodeBuilder, PropertyArguments arguments) : CommonCodeBuilder(commonCodeBuilder)
{
    private readonly PropertyArguments _arguments = arguments;

    public override void Build(IndentedWriter writer)
    {
        writer.WriteLine(BuildProperty());
    }

    private string BuildProperty()
    {
        var sb = new StringBuilder(_arguments.Accessibility.Value());

        if (_arguments.ReturnType == null)
            sb.Append(" string");
        else
            sb.Append(' ').Append(_arguments.ReturnType.ToString());

        sb.Append(' ').Append(_arguments.Name);
        if (_arguments.Getter)
        {
            sb.Append(" {");
            sb.Append(" get;");
            if (_arguments.Setter)
                sb.Append(" set;");
            sb.Append(" }");

            if (_arguments.Initial != null)
                sb.Append(" = ").Append(_arguments.Initial).Append(';');
        }
        else if (_arguments.Initial != null)
            sb.Append(" => ").Append(_arguments.Initial ?? "null").Append(';');

        return sb.ToString();
    }
}
