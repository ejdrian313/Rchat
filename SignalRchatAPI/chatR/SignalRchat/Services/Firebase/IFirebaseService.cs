namespace SignalRchat.Services.Firebase
{
    public interface IFirebaseService
    {
         bool Send(string token, string message);
    }
}