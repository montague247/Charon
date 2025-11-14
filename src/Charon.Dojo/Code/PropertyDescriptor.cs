namespace Charon.Dojo.Code;

public sealed class PropertyDescriptor
{
    private readonly PropertyArguments _arguments;
    private readonly CodeFileArguments _codeFileArguments;

    public PropertyDescriptor(string name, CodeFileArguments codeFileArguments, out PropertyArguments arguments)
    {
        _arguments = arguments = new(name);
        _codeFileArguments = codeFileArguments;
    }

    public PropertyDescriptor Accessibility(Accessibility accessibility)
    {
        _arguments.Accessibility = accessibility;

        return this;
    }

    public PropertyDescriptor ReturnType<T>()
    {
        return ReturnType(typeof(T));
    }

    public PropertyDescriptor ReturnType(Type type)
    {
        if (!TypeName.IsSimple(type))
            _codeFileArguments.AddUsing(type.Namespace!);

        _arguments.ReturnType = new TypeBlock(_codeFileArguments.TypeName, type);

        return this;
    }

    public PropertyDescriptor Getter(bool getter)
    {
        _arguments.Getter = getter;

        return this;
    }

    public PropertyDescriptor Setter(bool setter)
    {
        _arguments.Setter = setter;

        return this;
    }

    public PropertyDescriptor Initial(string? initial)
    {
        _arguments.Initial = initial;

        return this;
    }
}
