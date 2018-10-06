using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SignalRchat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Controllers
{
    public class DashboardController : BaseController
    {
        
        public DashboardController(IMongoClient mongo) : base(mongo)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllMessages()
        {
            return Ok(_context.GetCollection<Message>("Messages").Aggregate().ToList());
        }
    }
}
