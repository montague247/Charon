namespace Charon.Security
{
    public interface IPublicKeyRetriever
    {
        string DefaultStage { get; }

        byte[]? GetKey(string stage);
    }
}
