namespace Charon.Security
{
    public interface IPrivateKeyRetriever
    {
        byte[]? GetKey();

        byte[]? GetHash();
    }
}
