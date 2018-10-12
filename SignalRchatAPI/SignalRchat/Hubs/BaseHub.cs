using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Services.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
