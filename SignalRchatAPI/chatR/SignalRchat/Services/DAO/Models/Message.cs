using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SignalRchat.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }
    }
}
