using Microsoft.AspNetCore.DataProtection;

namespace SignalRchat.Services.Authentication
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        public CipherService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }
        public string Encrypt(string password)
        {
            var protector = _dataProtectionProvider.CreateProtector(AppSettingsService.CipherKey);
            var hash = protector.Protect(password);
            return hash;
        }
        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(AppSettingsService.CipherKey);
            var response = protector.Unprotect(cipherText);
            return response;
        }
    }
}
