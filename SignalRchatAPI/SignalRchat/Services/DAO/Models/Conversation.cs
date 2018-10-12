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
        [BsonElement("id")]
        [BsonRepresentation(BsonType.String)]
        public int Id { get; set; }

        [BsonElement("participants")]
        [BsonRepresentation(BsonType.Array)]
        public ICollection<User> Participants { get; set; }

        [BsonElement("participants")]
        [BsonRepresentation(BsonType.Array)]
        public ICollection<Message> Messages { get; set; }
    }
}
