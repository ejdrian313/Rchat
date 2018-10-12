using AutoMapper;
using FitPimp.Api.Services;
using FitPimp.Api.Services.Email;
using FitPimp.Api.ViewModels.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
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
        private readonly IGenerator _generator;
        private readonly IHostingEnvironment _environment;

        public AuthenticationService(
            IMongoClient context,
            ICipherService cipherService,
            ITokenService tokenService,
            IMapper mapper,
            IGenerator generator,
            IHostingEnvironment environment) : base(context, mapper)
        {
            _cipherService = cipherService;
            _tokenService = tokenService;
            _generator = generator;
            _environment = environment;
        }


        public IActionResult Login(LoginVm model)
        {
            try
            {
                var user = _context.GetDatabase("chatrdb").GetCollection<User>("Users").Aggregate().First().

                if (user == null)
                    return new BadRequestObjectResult("Not authorized");

                if (!user.IsA)
                    return new BadRequestObjectResult("Not authorized");

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
                else
                    return new BadRequestObjectResult("Not authorized");

                var token = _tokenService.Generate(user);

                _context.Upda();

                return  Ok(new TokenVm
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

        public async Task<GlobalServiceModel> ActivateAccount(string securityToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.SecurityToken == securityToken && !u.IsActive);

                if (user == null)
                    return new GlobalServiceModel(ServiceResult.NotFound, "User not found");

                if (user.IsDeleted)
                    return new GlobalServiceModel(ServiceResult.BadRequest, "User is deleted");

                user.IsActive = true;
                user.SecurityToken = null;

                // TODO Adding MainPost To UsersProfile
                user.Posts.Add(new Post());

                await _context.SaveChangesAsync();

                return new GlobalServiceModel();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex}");
                return new GlobalServiceModel(ServiceResult.Exception);
            }
        }

        public async Task<GlobalServiceModel<TokenVm>> ForgotPassword(ForgotPasswordVm model)
        {
            try
            {
                model.Email = model.Email.ToLower();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                    return new GlobalServiceModel<TokenVm>(ServiceResult.NotFound, "User not found");

                if (user.IsDeleted)
                    return new GlobalServiceModel<TokenVm>(ServiceResult.BadRequest, "User is deleted");

                user.SecurityToken = _generator.GenerateToken();

                if (!_environment.IsDevelopment())
                {
                    var bodyEmail = _emailMessageService.ForgotPasswordMessage(user.Name, user.SecurityToken);
                    var subjectEmail = _emailMessageService.ForgotPasswordSubject();

                    await _emailService.SendEmailAsync(user.Email, subjectEmail, bodyEmail);
                }

                await _context.SaveChangesAsync();

                if (_environment.IsDevelopment())
                    return new GlobalServiceModel<TokenVm>(new TokenVm
                    {
                        SecurityToken = user.SecurityToken
                    });

                return new GlobalServiceModel<TokenVm>();
            }
            catch (Exception e)
            {
                _logger.Error("ForgotPassword error " + e.Message);
                return new GlobalServiceModel<TokenVm>(ServiceResult.Exception);
            }
        }

        public async Task<GlobalServiceModel> ChangePassword(SetNewPasswordVm model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.SecurityToken == model.SecurityToken);

                if (user == null)
                    return new GlobalServiceModel(ServiceResult.NotFound, "User not found");

                if (user.IsDeleted)
                    return new GlobalServiceModel(ServiceResult.BadRequest, "User is deleted");

                user.Password = _cipherService.Encrypt(model.NewPassword);
                user.IsActive = true;
                user.SecurityToken = null;

                await _context.SaveChangesAsync();

                return new GlobalServiceModel();
            }
            catch (Exception e)
            {
                _logger.Error("ForgotPassword error " + e.Message);
                return new GlobalServiceModel(ServiceResult.Exception);
            }
        }
    }
}
