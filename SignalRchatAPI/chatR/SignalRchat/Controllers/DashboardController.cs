using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRchat.Services.DAO.Models;
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
        public IActionResult GetAllMessages()
        {
            return Ok(_context.Messages.Aggregate().ToList());
        }

        [HttpGet]
        public IActionResult Conversations()
        {
            try
            {
                var uid = UserId();
                var all = _context.Conversations.Aggregate().ToList();
                var userConversations = new List<Conversation>();
                foreach (var conversation in all)
                {
                    if (conversation.UserId.Contains(uid))
                    {
                        userConversations.Add(conversation);
                    }
                }
                return Ok(userConversations);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return BadRequest("Error occured");
            }
        }
    }
}
