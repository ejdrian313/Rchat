using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Controllers
{

    [EnableCors("CorsPolicy")]
    [Route("api/[Controller]/[Action]")]
    public class BaseController : Controller
    {
        protected readonly IMongoDatabase _context;
        protected readonly ILogger _logger;

        public BaseController(IMongoClient context)
        {
            _context = context.GetDatabase("chatrdb");
            _logger = LogManager.GetCurrentClassLogger();
        }
    }
}
