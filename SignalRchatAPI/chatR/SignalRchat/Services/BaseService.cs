using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Services.DAO;

namespace SignalRchat.Services
{
    public class BaseService
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;

        public BaseService(IOptions<Settings> options, IMongoClient context)
        {
            _context = new AppDbContext(options, context);
            _logger = LogManager.GetCurrentClassLogger();
        }
    }
}
