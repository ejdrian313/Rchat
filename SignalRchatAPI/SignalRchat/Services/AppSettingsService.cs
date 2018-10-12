using Microsoft.Extensions.Configuration;

namespace SignalRchat.Services
{
    public class AppSettingsService
    {
        private readonly IConfiguration _config;
        public static IConfigurationRoot Configuration { get; set; }
        public AppSettingsService(IConfiguration config)
        {
            _config = config;
        }
        public static string DbConnection => Configuration["ConnectionStrings:DefaultConnection"];

        public static string Secret => Configuration["AppSettings:Authentication:Secret"];
        public static string IssuserName => Configuration["AppSettings:Authentication:IssuserName"];
        public static string AudienceName => Configuration["AppSettings:Authentication:AudienceName"];
        public static string ExpiryInMinutes => Configuration["AppSettings:Authentication:ExpiryInMinutes"];

        public static string CipherKey => Configuration["AppSettings:CipherKey"];

    }
}
