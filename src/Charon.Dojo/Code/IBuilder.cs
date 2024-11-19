namespace Charon.Dojo.Code
{
    public interface IBuilder
    {
        string Build();
        void Build(IndentedWriter writer);
    }
}
