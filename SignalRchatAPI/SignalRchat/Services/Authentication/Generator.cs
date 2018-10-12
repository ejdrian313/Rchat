using System;
using System.Linq;

namespace SignalRchat.Services.Authentication
{
    public class Generator : IGenerator
    {
        private const int PasswordLength = 10;
        private const int TokenLength = 80;
        private const string PasswordCharArray = "abcdefghijklmnopABCDEFGHIJKLMNOP0123456789!#$&";
        private const string TokenCharArray = "abcdefghijklmnopABCDEFGHIJKLMNOP0123456789-";

        public string GeneratePassword() => Generate(PasswordCharArray, PasswordLength);

        public string GenerateToken() => Generate(TokenCharArray, TokenLength);

        private static string Generate(string charArray, int number)
        {
            var random = new Random();

            return new string(Enumerable.Repeat(charArray, number)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}
