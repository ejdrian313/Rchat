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
    }
}
