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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRchat.Hubs
{
    //
    public class ChatHub : BaseHub
    {
    //     private readonly AppDbContext _context;
    //     private readonly ILogger _logger;

        public ChatHub(IOptions<Settings> options, IMongoClient context) : base(options, context)
        {
            // _context = new AppDbContext(options, context);
            // _logger = LogManager.GetCurrentClassLogger();
        }

        // protected string UserName() => Context.User.FindFirstValue(TokenClaim.UserName) ?? "";
        
        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add($"{Context.ConnectionId} {UserName()}");

            Clients.All.SendAsync("currentConnections", UserHandler.ConnectedIds.Count());
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove($"{Context.ConnectionId} {UserName()}");
            Clients.All.SendAsync("currentConnections", UserHandler.ConnectedIds.Count());
            return base.OnDisconnectedAsync(exception);
        }
        
        [Authorize]
        public void Send(string message)
        {
            var conv = _context.Messages.Aggregate().ToList();

            conv.Add(
              new Message
              {
                  Name = UserName(),
                  Body = message
              });

            Clients.All.SendAsync("broadcastMessage", UserName(), message);
        }
    }
}