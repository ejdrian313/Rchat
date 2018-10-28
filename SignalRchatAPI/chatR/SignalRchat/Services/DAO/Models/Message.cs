using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO.Models
{
    public class Message
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public string ConversationId { get; set; } 
    }
}
