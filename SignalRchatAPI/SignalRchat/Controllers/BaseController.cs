﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Services.DAO;
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
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;

        public BaseController(IOptions<Settings> options, IMongoClient context)
        {
            _context = new AppDbContext(options, context);
            _logger = LogManager.GetCurrentClassLogger();
        }
    }
}
