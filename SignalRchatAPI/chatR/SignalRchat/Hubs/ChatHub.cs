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
using SignalRchat.Services;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Services.Helpers;

namespace SignalRchat.Hubs {

    [Authorize]
    public class ChatHub : BaseHub {
        public ChatHub (IOptions<Settings> options, IMongoClient context) : base (options, context) 
        { 
        }

        public override Task OnConnectedAsync () 
        {
            UserHandler.ConnectedIds.Add ($"{Context.ConnectionId} {UserName()}");

            Clients.All.SendAsync ("currentConnections", UserHandler.ConnectedIds.Count ());
            return base.OnConnectedAsync ();
        }

        public override Task OnDisconnectedAsync (Exception exception) 
        {
            UserHandler.ConnectedIds.Remove ($"{Context.ConnectionId} {UserName()}");
            Clients.All.SendAsync ("currentConnections", UserHandler.ConnectedIds.Count ());
            return base.OnDisconnectedAsync (exception);
        }

        public void Send (string message) 
        {
            _context.Messages.InsertOne (
                new Message {
                    Name = UserName (),
                    Body = message
                });

            Clients.All.SendAsync ("broadcastMessage", UserName (), message);
        }
    }
}