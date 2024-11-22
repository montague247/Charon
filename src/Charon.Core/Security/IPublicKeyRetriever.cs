namespace Charon.Security
{
    public interface IPublicKeyRetriever
    {
        byte[] GetKey(string stage);
    }
}
