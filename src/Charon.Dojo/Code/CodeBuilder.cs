namespace Charon.Dojo.Code;

public sealed class CodeBuilder(CommonCodeBuilder commonCodeBuilder) : CommonCodeBuilder(commonCodeBuilder)
{
    private readonly List<object[]?> _lines = [];
    private int _level;

    public CodeBuilder Indent()
    {
        _level++;

        return this;
    }

    public CodeBuilder Indent(Action<CodeBuilder> indentedCode)
    {
        Indent();

        indentedCode(this);

        return Unindent();
    }

    public CodeBuilder Unindent()
    {
        _level--;

        return this;
    }

    public CodeBuilder NewLine()
    {
        _lines.Add(null);

        return this;
    }

    public CodeBuilder WriteLine(string value)
    {
        if (_level > 0)
            _lines.Add([string.Empty.PadRight(_level * 4), value]);
        else
            _lines.Add([value]);

        return this;
    }

    public CodeBuilder WriteLine(params object[] parts)
    {
        if (_level > 0)
        {
            var extParts = new object[parts.Length + 1];
            Array.Copy(parts, 0, extParts, 1, parts.Length);
            extParts[0] = string.Empty.PadRight(_level * 4);

            _lines.Add(extParts);
        }
        else
            _lines.Add(parts);

        return this;
    }

    public CodeBuilder StartBlock()
    {
        return WriteLine("{").Indent();
    }

    public CodeBuilder EndBlock(char? ending = null)
    {
        return Unindent().WriteLine($"}}{ending}");
    }

    public CodeBuilder StartSwitch(string prefix, string name)
    {
        return WriteLine(prefix, ' ', name, " switch").StartBlock();
    }

    public CodeBuilder EndSwitch()
    {
        return EndBlock(';');
    }

    public override void Build(IndentedWriter writer)
    {
        foreach (var parts in _lines)
        {
            if (parts == null)
                writer.NewLine();
            else
                writer.WriteLine(string.Join(null, parts));
        }
    }
}
