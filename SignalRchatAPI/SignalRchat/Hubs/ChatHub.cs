using Microsoft.AspNetCore.SignalR;
using SignalRchat.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Hubs
{
    public class ChatHub : Hub
    {
        public List<Message> messages;

        ChatHub() : base()
        {
            messages = new List<Message>();
        }

        public void Send(string name, string message)
        {
            messages.Add(new Message
            {
                Name = name,
                Body = message
            });

            Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}