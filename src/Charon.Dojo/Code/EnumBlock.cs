namespace Charon.Dojo.Code;

public sealed class EnumBlock(TypeName typeName, Enum e) : ICodeBlock
{
    private readonly TypeName _typeName = typeName;
    private readonly Enum _enum = e;

    public override string ToString()
    {
        return _typeName.GetName(_enum)!;
    }
}
