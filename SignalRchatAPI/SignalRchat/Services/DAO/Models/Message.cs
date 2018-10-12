using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SignalRchat.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO.Models
{
    [BsonCollection("Messages")]
    public class Message
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }
    }
}
