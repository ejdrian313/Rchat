using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SignalRchat.Helpers;
using SignalRchat.Services;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO;
using SignalRchat.Services.DAO.Models;

namespace SignalRchat.Hubs {

    [Authorize]
    public class ChatHub : BaseHub 
    {
        public ChatHub (IOptions<Settings> options, IMongoClient context) : base (options, context) 
        { 
        }

        public override Task OnConnectedAsync () 
        {
            if (UserHandler.ConnectedIdHubAndIdUser.ContainsValue(UserId())) 
            {
                var connected = UserHandler.ConnectedIdHubAndIdUser.First(c => c.Value == UserId());
                UserHandler.ConnectedIdHubAndIdUser.Remove(connected.Key);
            } 
            UserHandler.ConnectedIdHubAndIdUser.Add(Context.ConnectionId, UserId());
            Clients.All.SendAsync ("currentConnections", UserHandler.ConnectedIdHubAndIdUser.Count ());
            return base.OnConnectedAsync ();
        }

        public override Task OnDisconnectedAsync (Exception exception) 
        {
            UserHandler.ConnectedIdHubAndIdUser.Remove(Context.ConnectionId);
            Clients.All.SendAsync ("currentConnections", UserHandler.ConnectedIdHubAndIdUser.Count ());
            return base.OnDisconnectedAsync (exception);
        }

        public void Send(string message, string conversationId) 
        {
            try 
            {
                var conversation = _context.Conversations.Aggregate().ToList().FirstOrDefault(c => c.Id.ToString().Equals(conversationId));
                if (conversation == null) 
                {
                    _logger.Warn($"Conversation {conversationId} does not exist");
                    return;
                }

                var messageVm = new Message {
                        Name = UserName(),
                        Body = message,
                        ConversationId = conversationId
                    };
                
                _context.Messages.InsertOneAsync(messageVm);
            
                var userIds = conversation.UserId.ToList();

                UserHandler.ConnectedIdHubAndIdUser.ToList().ForEach(e => _logger.Info($"Key:{e.Key}  Value:{e.Value}"));

                if (userIds.Any()) 
                {
                    userIds.ForEach( 
                        userId => {
                            var hubId = UserHandler.ConnectedIdHubAndIdUser.FirstOrDefault(c => c.Value.Equals(userId));
                            if (hubId.Key != null) { //send if connection exists
                                Clients.Client(hubId.Key).SendAsync("broadcastMessage", messageVm);
                            }
                        }
                    );
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}