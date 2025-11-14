namespace Charon.Dojo.Code;

public sealed class TypeBlock(TypeName typeName, Type type) : ICodeBlock
{
    private readonly TypeName _typeName = typeName;
    private readonly Type _type = type;

    public override string ToString()
    {
        return _typeName.GetName(_type)!;
    }
}
