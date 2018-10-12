using AutoMapper;
using MongoDB.Driver;
using NLog;

namespace SignalRchat.Services.Authentication
{
    public class BaseService
    {
        protected readonly IMongoClient _context;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;

        public BaseService(IMongoClient context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }
    }
}
