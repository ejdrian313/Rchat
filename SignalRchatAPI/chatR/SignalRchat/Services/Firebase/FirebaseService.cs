using FCM.Net;

namespace SignalRchat.Services.Firebase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly Sender _sender;
        private readonly AppSettingsService _settings;

        FirebaseService(AppSettingsService service) {
            _settings = service;
        }

        public bool Send(string token, string message)
        {
            var servce = new Sender("KEY");
             

            return true;
        }
    }
}