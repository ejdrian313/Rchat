using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SignalRchat.Services.DAO;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Services.DAO.ViewModels;
using System;
using System.Threading.Tasks;

namespace SignalRchat.Services.Authentication
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly ICipherService _cipherService;
        private readonly ITokenService _tokenService;
 

        public AuthenticationService(
            IOptions<Settings> options,
            IMongoClient context,
            ICipherService cipherService,
            ITokenService tokenService) : base(options, context)
        {
            _cipherService = cipherService;
            _tokenService = tokenService;

        }

        public IActionResult Login(LoginVm model)
        {
            try
            {
                var user = _context.Users.Find(c => c.Email == model.Email).FirstOrDefault();

                if (user == null)
                    return new BadRequestObjectResult("Not authorized");

                if (!user.IsActive)
                    return new BadRequestObjectResult("User is not active");

                if (user.IsDeleted)
                    return new BadRequestObjectResult("Not authorized");

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    if (string.IsNullOrWhiteSpace(user.Password))
                        return new BadRequestObjectResult("Not authorized");

                    var decryptedPassword = _cipherService.Decrypt(user.Password);

                    if (decryptedPassword != model.Password)
                        return new BadRequestObjectResult("Not authorized");
                }
                else return new BadRequestObjectResult("Not authorized");

                var token = _tokenService.Generate(user);

                return new OkObjectResult(new TokenVm
                {
                    Token = token
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Error: {e}");
                return new BadRequestObjectResult("Error occured");
            }
        }
    }
}
