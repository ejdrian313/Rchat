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

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FcmToken { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        public string Salt { get; internal set; }
    }
}
