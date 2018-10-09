using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using SignalRchat.Models;
using SignalRchat.Services;
using SignalRchat.Services.DAO;
using SignalRchat.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Hubs
{
    public class ChatHub : Hub
    {
        private IMongoClient _mongo;
        
        public ChatHub(IMongoClient mongo) : base()
        {
            _mongo = mongo;
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
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
            _mongo.GetDatabase("chatrdb").GetCollection<Message>("Messages").InsertOneAsync(new Message
            {
                Name = name,
                Body = message
            });

            Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}