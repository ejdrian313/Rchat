using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalRchat.Services.Authentication;
using SignalRchat.Services.DAO.Models;
using SignalRchat.Services.DAO.ViewModels;

namespace SignalRchat.Controllers
{
    public class AuthenticationController : BaseController
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly ICipherService _cipherService; 

        public AuthenticationController(IOptions<Settings> options, IMongoClient context, IAuthenticationService authenticationService, ICipherService cipherService) : base(options, context)
        {
            _authenticationService = authenticationService;
            _cipherService = cipherService;

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(LoginVm loginVm)
        {
            if (!ModelState.IsValid) return BadRequest("Model state not valid");

            var response = _authenticationService.Login(loginVm);

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            _context.Users.InsertMany(new List<User>
            {
                new User
                {
                    Name = "filu34",
                    Email = "filu34@gmail.com",

                    Password = _cipherService.Encrypt("Admin123"),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    Name = "ejdrian313",
                    Email = "adrian.kujawski.313@gmail.pl",

                    Password = _cipherService.Encrypt("12345678"),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                },

            });

            return Ok();
        }
    }
}
