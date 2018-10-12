using Microsoft.IdentityModel.Tokens;
using SignalRchat.Services.DAO.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRchat.Services.Authentication
{
    public class TokenService : ITokenService
    {
        public string Generate(User user)
        {
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: AppSettingsService.IssuserName,
                audience: AppSettingsService.AudienceName,
                claims: new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(TokenClaim.UserId, user.Id.ToString()),
                    new Claim(TokenClaim.UserName, user.Name)
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(AppSettingsService.ExpiryInMinutes)),
                signingCredentials: new SigningCredentials(
                    key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppSettingsService.Secret)),
                    algorithm: SecurityAlgorithms.HmacSha256)));
        }
    }

    public static class TokenClaim
    {
        public static readonly string UserId = "userId";
        public static readonly string UserRoleId = "userRoleId";
        public static readonly string UserName = "userName";

    }
}
