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
                    var users = context.GetDatabase("chatrdb").GetCollection<User>("users");


                    if (!users.Aggregate().Any())
                        Users().ForEach( user => users.InsertOne(user) );
                  
                }
                catch (Exception ex)
                {
                    var services = serviceScope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }


        private static List<User> Users()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "filu34",
                    Email = "filu34@gmail.com",

                    Password = _cipherService.Encrypt("Admin123"),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    Id = 2,
                    Name = "ejdrian313",
                    Email = "adrian.kujawski.313@gmail.com",
                
                    Password = _cipherService.Encrypt("12345678"),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
              
            };
        }
    }
}
