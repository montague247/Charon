namespace Charon.Security
{
    public interface IPrivateKeyRetriever
    {
        string DefaultStage { get; }

        byte[] GetKey();

        byte[] GetHash();
    }
}
