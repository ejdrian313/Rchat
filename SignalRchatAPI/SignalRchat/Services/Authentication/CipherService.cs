using Microsoft.AspNetCore.DataProtection;

namespace SignalRchat.Services.Authentication
{
    public class CipherService : ICipherService
    {
        public CipherService()
        {
        }

        public string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.SaltRevision.Revision2Y); 
        }
        public bool Verify(string userSubmittedPassword, string hashedPassword)
        {
            bool validPassword = BCrypt.Net.BCrypt.Verify(userSubmittedPassword, hashedPassword);
            return validPassword;
        }
    }
}
