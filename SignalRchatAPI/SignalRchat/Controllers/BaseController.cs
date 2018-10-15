using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO;
using System.Security.Claims;

namespace SignalRchat.Controllers
{
    [Authorize]
    [EnableCors("CorsPolicy")]
    [Route("api/[Controller]/[Action]")]
    public class BaseController : Controller
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;

        public BaseController(IOptions<Settings> options, IMongoClient context)
        {
            _context = new AppDbContext(options, context);
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected string UserId() => User.FindFirstValue(TokenClaim.UserId) ?? "";
        protected string UserName() => User.FindFirstValue(TokenClaim.UserName) ?? "";

    }
}
