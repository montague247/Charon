using Charon.IO;

namespace Charon.Dojo.Code;

public sealed class CodeWriter
{
    private readonly CodeFileArguments _arguments;
    private ClassBuilder? _classBuilder;

    public CodeWriter(Action<CodeFileDescriptor> descriptor)
    {
        _arguments = new();
        descriptor(new CodeFileDescriptor(out _arguments));
    }

    public ClassBuilder Class(string name, Action<ClassDescriptor> descriptor)
    {
        descriptor(new ClassDescriptor(name, _arguments, out ClassArguments classArguments));

        _classBuilder = new ClassBuilder(this, _arguments, classArguments);

        return _classBuilder;
    }

    public string Build()
    {
        ArgumentNullException.ThrowIfNull(_arguments.Usings);

        _arguments.Usings.Remove(_arguments.NamespaceName!);

        using var writer = new IndentedWriter();

        foreach (var name in _arguments.Usings.OrderBy(s => s))
        {
            writer.WriteLine($"using {name};");
        }

        if (_arguments.Usings.Count > 0)
            writer.NewLine();

        writer.WriteLine($"namespace {_arguments.NamespaceName}");
        writer.WriteLine("{");
        writer.Indent();

        _classBuilder?.Build(writer);

        writer.Unindent();
        writer.WriteLine("}");

        return writer.Text;
    }

    public void ToFile(string path, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var tempPath = string.Concat(path, ".temp");

        File.WriteAllText(tempPath, Build());

        FileComparer.Move(tempPath, path, cancellationToken);
    }
}
