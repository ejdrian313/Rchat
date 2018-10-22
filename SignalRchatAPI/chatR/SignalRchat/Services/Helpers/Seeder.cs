using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.Helpers
{
    public static class Seeder
    {
        private static ICipherService _cipherService;
        public static void SeedDatabase(this IApplicationBuilder applicationBuilder, ICipherService cipherService)
        {
            _cipherService = cipherService;

            using (var serviceScope = applicationBuilder.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                try
                {
                    var context = serviceScope.ServiceProvider.GetService<IMongoClient>();
                    var databse = context.GetDatabase("chatrdb");
                    var users = databse.GetCollection<User>("users");
                    var conversations = databse.GetCollection<Conversation>("conversations");
                    var messages = databse.GetCollection<Message>("messages");

                    if (!messages.Aggregate().Any())
                        Messages().ForEach(m => messages.InsertOne(m));

                    if (!users.Aggregate().Any())
                    {
                        Users().ForEach(user => users.InsertOne(user));
                        var userList = users.Aggregate().ToList();


                        if (!conversations.Aggregate().Any())
                            Conversations(userList[0].Id.ToString(), userList[1].Id.ToString()).ForEach(conversation => conversations.InsertOne(conversation));

                      

                    }

                }
                catch (Exception ex)
                {
                    var services = serviceScope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }

        private static List<Conversation> Conversations(string id1, string id2)
        {
            return new List<Conversation>
            {
                new Conversation
                {
                   Id = Guid.NewGuid(),
                   UserId = new List<String>
                   {
                        id1,
                        id2
                   },
                   Messages = new List<Message>
                   {
                       new Message
                       {
                           Id = Guid.NewGuid(),
                           Body = "test message",
                           Name = "filu34"
                       },
                       new Message
                       {
                           Id = Guid.NewGuid(),
                           Body = "HUO HUO",
                           Name = "ejdrian313"
                       },
                       new Message
                       {
                           Id = Guid.NewGuid(),
                           Body = "test 3",
                           Name = "ejdrian313"
                       },
                   }
                }
            };
        }

         private static List<Message> Messages()
        {
            return new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    Name = "filu34",
                    Body = "test"
                }
            };
        }

        private static List<User> Users()
        {
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "filu34",
                    Email = "filu34@gmail.com",

                    Password = _cipherService.Encrypt("Admin123"),
                    Salt = BCrypt.Net.BCrypt.GenerateSalt(30),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "ejdrian313",
                    Email = "adrian.kujawski.313@gmail.com",
                
                    Password = _cipherService.Encrypt("12345678"),
                    Salt = BCrypt.Net.BCrypt.GenerateSalt(30),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
               new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Huo",
                    Email = "a3@gmail.com",

                    Password = _cipherService.Encrypt("Admin123"),
                    Salt = BCrypt.Net.BCrypt.GenerateSalt(30),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Buk",
                    Email = "a@a.pl",

                    Password = _cipherService.Encrypt("Admin123"),
                    Salt = BCrypt.Net.BCrypt.GenerateSalt(30),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
            };
        }
    }
}
