namespace SignalRchat.Services.Authentication
{
    public interface ICipherService
    {
        string Encrypt(string password);
        bool Verify(string userSubmittedPassword, string hashedPassword);
    }
}
