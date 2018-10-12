using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO.Models
{
    public class User
    {

        public User()
        {
            CreationDate = DateTime.Now;
        }

        [BsonId]
        [BsonElement("id")]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("fcmtoken")]
        public string FcmToken { get; set; }

        [BsonElement("creationdate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("isactive")]
        public bool IsActive { get; set; }

        [BsonElement("isdeleted")]
        public bool IsDeleted { get; set; }
    }
}
