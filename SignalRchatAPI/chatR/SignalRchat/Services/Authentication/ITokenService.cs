using SignalRchat.Services.DAO.Models;

namespace SignalRchat.Services.Authentication
{
    public interface ITokenService
    {
        string Generate(User user);
    }
}