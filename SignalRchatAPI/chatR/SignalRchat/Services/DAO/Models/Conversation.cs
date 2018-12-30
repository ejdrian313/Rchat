using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO.Models
{
    public class Conversation
    {
        [BsonId]
        public Guid Id { get; set; }

        public ICollection<String> UserId { get; set; }
    }
}
