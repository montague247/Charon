using System.Text;

namespace Charon.Dojo.Code;

public sealed class IndentedWriter : IDisposable
{
    private readonly StringBuilder _sb = new();
    private readonly StringWriter _writer;
    private int _level;

    public IndentedWriter()
    {
        _writer = new(_sb)
        {
            NewLine = "\n"
        };
    }

    public int Writer { get; set; }

    public IndentedWriter Indent()
    {
        _level++;

        return this;
    }

    public IndentedWriter Unindent()
    {
        _level--;

        return this;
    }

    public void NewLine()
    {
        _writer.WriteLine();
    }

    public void WriteLine(string value)
    {
        if (_level > 0)
            _writer.Write(string.Empty.PadRight(_level * 4));

        _writer.WriteLine(value);
    }

    public string Text { get { return _sb.ToString(); } }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
            _writer?.Dispose();
    }
}
