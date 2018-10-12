using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Controllers
{
    public class DashboardController : BaseController
    {
        
        public DashboardController(IOptions<Settings> options, IMongoClient mongo) : base(options, mongo)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllMessages()
        {
            return Ok(_context.Messages.Aggregate().ToList());
        }
    }
}
