using MongoDB.Driver;
using SignalRchat.Services.DAO;
using SignalRchat.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services
{
    public class ChatRRepository<T> : IChatRRepository<T> where T : class
    {
        public IMongoDatabase Database { get; }
        public ChatRRepository(IMongoClient client)
        {
            Database = client.GetDatabase("chatrdb");
        }
        public async Task InsertOne(T model)
        {
            var collectionName = GetCollectionName();
            var collection = Database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(model);
        }

        public IMongoCollection<T> GetCollection(T model)
        {
            var collectionName = GetCollectionName();
            var collection = Database.GetCollection<T>(collectionName);
            return collection;
        }

        private static string GetCollectionName()
        {
            return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute).CollectionName;
        }

        
    }
}
