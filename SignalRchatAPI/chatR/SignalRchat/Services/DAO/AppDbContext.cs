using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO
{
    public class AppDbContext
    {

        private readonly IMongoDatabase _db;
      
        public AppDbContext(IOptions<Settings> options, IMongoClient mongo)
        {
            _db = mongo.GetDatabase(options.Value.Database);
        }

        public IMongoCollection<User> Users => _db.GetCollection<User>("users");
        public IMongoCollection<Conversation> Conversations => _db.GetCollection<Conversation>("conversations");
        public IMongoCollection<Message> Messages => _db.GetCollection<Message>("messages");

    }
   
}
