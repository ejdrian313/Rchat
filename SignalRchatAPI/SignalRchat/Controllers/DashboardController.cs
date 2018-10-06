using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Controllers
{
    public class DashboardController : Controller
    {

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllMessages()
        {
            return Ok();
        }
    }
}
