namespace SignalRchat.Services.Authentication
{
    public interface ICipherService
    {
        string Encrypt(string password);
        string Decrypt(string cipherText);
    }
}
