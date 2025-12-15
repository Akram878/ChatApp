using System;
using System.Threading.Tasks;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class GoogleAuthService
    {
        public Task<string> AuthenticateAsync()
        {
            // Simulate OAuth delay
            return Task.Delay(1000).ContinueWith(t => "google_token_123");
        }
    }

    public class AppleAuthService
    {
        public Task<string> AuthenticateAsync()
        {
            // Simulate OAuth delay
            return Task.Delay(1000).ContinueWith(t => "apple_token_456");
        }
    }

    public class PasswordResetService
    {
        public Task SendResetLinkAsync(string email)
        {
            return Task.Delay(500); // Simulate network
        }
    }

    public class AuthService : IAuthService
    {
        private User _currentUser;
        private readonly GoogleAuthService _googleService = new GoogleAuthService();
        private readonly AppleAuthService _appleService = new AppleAuthService();
        private readonly PasswordResetService _resetService = new PasswordResetService();

        public User CurrentUser => _currentUser;

        public async Task<bool> LoginAsync(string email, string password)
        {
            await Task.Delay(500); // Simulate DB check
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                _currentUser = new User
                {
                    Email = email,
                    Username = email.Split('@')[0],
                    Profile = new UserProfile
                    {
                        Status = UserStatus.Online,
                        AvatarUrl = "/Assets/avatar_placeholder.png"
                    }
                };
                return true;
            }
            return false;
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            var token = await _googleService.AuthenticateAsync();
            if (token != null)
            {
                _currentUser = new User { Username = "GoogleUser", Email = "user@gmail.com" };
                return true;
            }
            return false;
        }

        public async Task<bool> LoginWithAppleAsync()
        {
            var token = await _appleService.AuthenticateAsync();
            if (token != null)
            {
                _currentUser = new User { Username = "AppleUser", Email = "user@icloud.com" };
                return true;
            }
            return false;
        }

        public async Task RegisterAsync(string email, string password, string username)
        {
            await Task.Delay(500); // Simulate API
            // Validation logic here
        }

        public async Task RecoverPasswordAsync(string email)
        {
            await _resetService.SendResetLinkAsync(email);
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}
