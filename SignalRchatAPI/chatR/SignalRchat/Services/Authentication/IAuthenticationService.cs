﻿using Microsoft.AspNetCore.Mvc;
using SignalRchat.Services.DAO.ViewModels;
using System.Threading.Tasks;

namespace SignalRchat.Services.Authentication
{
    public interface IAuthenticationService
    {
        IActionResult Login(LoginVm model);
    }
}