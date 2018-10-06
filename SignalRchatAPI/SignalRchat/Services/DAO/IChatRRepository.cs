using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Services.DAO
{
    public interface IChatRRepository<T> where T : class
    {
        Task InsertOne(T model);
        IMongoCollection<T> GetCollection(T model);
    }

}
