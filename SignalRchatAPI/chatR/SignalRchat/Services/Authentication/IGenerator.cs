namespace SignalRchat.Services.Authentication
{
    public interface IGenerator
    {
        string GeneratePassword();
        string GenerateToken();
    }
}
