using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO;
using System.Security.Claims;

namespace SignalRchat.Hubs
{
    public class BaseHub : Hub
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;

        public BaseHub(IOptions<Settings> options, IMongoClient context)
        {
            _context = new AppDbContext(options, context);
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected string UserName() => Context.User.FindFirstValue(TokenClaim.UserName) ?? "";

    }
}
