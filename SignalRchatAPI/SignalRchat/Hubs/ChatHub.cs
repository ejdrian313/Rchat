using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRchat.Services;
using SignalRchat.Services.DAO;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Hubs
{
    public class ChatHub : BaseHub
    {
        public ChatHub(IOptions<Settings> options, IMongoClient context) : base(options, context)
        {
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            _logger.Info(Context.ConnectionId);

            Clients.All.SendAsync("currentConnections", UserHandler.ConnectedIds.Count());
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.SendAsync("currentConnections", UserHandler.ConnectedIds.Count());
            return base.OnDisconnectedAsync(exception);
        }

        public void Send(string name, string message)
        {
            _context.Messages.InsertOneAsync(
                new Message
                {
                    Name = name,
                    Body = message
                });

            Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}