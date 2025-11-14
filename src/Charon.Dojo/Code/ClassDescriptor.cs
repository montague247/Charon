namespace Charon.Dojo.Code;

public sealed class ClassDescriptor
{
    private readonly ClassArguments _arguments;
    private readonly CodeFileArguments _codeFileArguments;

    public ClassDescriptor(string name, CodeFileArguments codeFileArguments, out ClassArguments arguments)
    {
        _arguments = arguments = new(name);
        _codeFileArguments = codeFileArguments;
    }

    public ClassDescriptor Accessibility(Accessibility accessibility)
    {
        _arguments.Accessibility = accessibility;

        return this;
    }

    public ClassDescriptor Inheritance(Inheritance inheritance)
    {
        _arguments.Inheritance = inheritance;

        return this;
    }

    public ClassDescriptor Partial(bool partial)
    {
        _arguments.Partial = partial;

        return this;
    }

    public ClassDescriptor Inherit<T>()
    {
        return Inherit(typeof(T));
    }

    public ClassDescriptor Inherit(Type type)
    {
        if (!TypeName.IsSimple(type))
            _codeFileArguments.AddUsing(type.Namespace!);

        _arguments.Inherits ??= [];
        _arguments.Inherits.Add(new TypeBlock(_codeFileArguments.TypeName, type));

        return this;
    }
}
