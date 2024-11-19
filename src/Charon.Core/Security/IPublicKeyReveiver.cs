namespace Charon.Security
{
    public interface IPublicKeyReveiver
    {
        byte[] GetKey(string stage);
    }
}
