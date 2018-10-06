using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using SignalRchat.Models;
using SignalRchat.Services;
using SignalRchat.Services.DAO;
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