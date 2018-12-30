using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Services.DAO.ViewModels;
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
        public IActionResult GetAllMessages(string conversationId)
        {
            try
            {
                var conversation = _context.Conversations.Aggregate().ToList().FirstOrDefault(c => c.UserId.Contains(UserId()));

                if (conversation == null) return BadRequest("Dont have access or conversation does not exists");
                
                var messages = _context.Messages.Find(m => m.ConversationId == conversationId).ToList();

                return Ok(messages);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return BadRequest(e);
            }
        }

        [HttpGet]
        public IActionResult Conversations()
        {
            try
            {
                var all = _context.Conversations.Aggregate().ToList();
                var userConversations = new List<ConversationsVm>();

                foreach (var conversation in all)
                {
                    if (conversation.UserId.Contains(UserId()))
                    {
                        var lastMessage = _context.Messages.Aggregate().ToList().LastOrDefault(m => m.ConversationId.Equals(conversation.Id.ToString()));

                        var foreign = conversation.UserId.FirstOrDefault(c => c != UserId());
                        var name = "Yourself";
                        if (foreign != null) 
                        {
                            name = _context.Users.Aggregate().ToList().FirstOrDefault(u => u.Id.ToString().Equals(foreign)).Name;
                        }
                        
                        if (lastMessage == null) 
                        {
                           userConversations.Add(new ConversationsVm {
                                Id = conversation.Id.ToString(),
                                NameForeign = name,
                                LastMessage = $"Type to your friend"
                            });
                        }
                        else
                        {
                            userConversations.Add(new ConversationsVm {
                                Id = conversation.Id.ToString(),
                                NameForeign = name,
                                LastMessage = $"{lastMessage.Name}: {lastMessage.Body}"
                            });
                        }
                        
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

        [HttpPost]
        public IActionResult Create([FromBody]CreateConversationVm createVm)
        {
            if (!ModelState.IsValid) return BadRequest("Model state not valid");

            var user = _context.Users.Aggregate().ToList().FirstOrDefault(u => u.Email == createVm.Email);
            if (user == null) return BadRequest("User does not exist");

            if (user.Id.ToString().Equals(UserId())) 
            {
                 _context.Conversations.InsertOneAsync(new Conversation
                    {
                        UserId = new string[] {
                            UserId()
                        }
                    });
                return Ok();
            } 

            var convs = _context.Conversations.Aggregate().ToList();
            var conversation = convs.FirstOrDefault(c => c.UserId.Contains(UserId()) && c.UserId.Contains(user.Id.ToString()));
            if(conversation == null) {
                _context.Conversations.InsertOneAsync(new Conversation
                    {
                        UserId = new string[] {
                            UserId(),
                            user.Id.ToString()
                        }
                    });
                return Ok();
            } else {
                return BadRequest("Conversation exists");
            }
        }
    }
}
