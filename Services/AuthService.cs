using System;
using System.Threading.Tasks;
using ChatApp.Models;
using System.Collections.Generic;
namespace ChatApp.Services
{
    public class GoogleAuthService : IExternalAuthProvider
    {
        public string ProviderName => "Google";

        public async Task<User> AuthenticateAsync()
        {
            // Simulate OAuth delay
            await Task.Delay(500);
            return new User
            {
                Username = "GoogleUser",
                Email = "user@gmail.com",
                Profile = new UserProfile
                {
                    Status = UserStatus.Online,
                    AvatarUrl = "/Assets/google.png",
                    Bio = "Signed in with Google"
                }
            };
        }
    }

    public class AppleAuthService : IExternalAuthProvider
    {
        public string ProviderName => "Apple";

        public async Task<User> AuthenticateAsync()
        {
            await Task.Delay(500);
            return new User
            {
                Username = "AppleUser",
                Email = "user@icloud.com",
                Profile = new UserProfile
                {
                    Status = UserStatus.Online,
                    AvatarUrl = "/Assets/apple.png",
                    Bio = "Signed in with Apple"
                }
            };
        }
    }

    public class PasswordResetService : IPasswordResetService
    {
        public async Task<string> GenerateResetTokenAsync(string email)
        {
            await Task.Delay(200);
            return $"token-{Guid.NewGuid():N}";
        }

        public async Task<bool> SendResetLinkAsync(string email, string token)
        {
            await Task.Delay(200);
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(token);
        }

        public Task<bool> ValidateResetTokenAsync(string email, string token)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(token));
        }
    }

    public class AuthService : IAuthService
    {
        private readonly IPasswordResetService _resetService;
        private readonly Dictionary<string, string> _userPasswords = new();
        private readonly Dictionary<string, User> _userDirectory = new();
        private User? _currentUser;
        public User? CurrentUser => _currentUser;


        public AuthService(IPasswordResetService resetService)
        {
            _resetService = resetService;

            var seededUser = new User
            {
                Email = "demo@chatapp.com",
                Username = "DemoUser",
                Profile = new UserProfile
                {
                    Status = UserStatus.Online,
                    AvatarUrl = "/Assets/avatar_placeholder.png",
                    Bio = "Demo account"
                }
            };

            _userDirectory[seededUser.Email] = seededUser;
            _userPasswords[seededUser.Email] = "password";
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            await Task.Delay(500); // Simulate DB check
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (_userPasswords.TryGetValue(email, out var storedPassword) && storedPassword == password)
            {
                _currentUser = _userDirectory[email];
                _currentUser.Profile.Status = UserStatus.Online;
                return true;
            }
            return false;
        }

        public async Task<bool> LoginWithProviderAsync(IExternalAuthProvider provider)
        {
            var user = await provider.AuthenticateAsync();
            if (user != null)
            {
                _currentUser = user;
                if (!_userDirectory.ContainsKey(user.Email))
                {
                    _userDirectory[user.Email] = user;
                    _userPasswords[user.Email] = "oauth-login";
                }
                return true;
            }
            return false;
        }

        public async Task RegisterAsync(string email, string password, string username)
        {
            await Task.Delay(500); // Simulate API
            // Validation logic here

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Email, password, and username are required");
            }

            if (_userDirectory.ContainsKey(email))
            {
                throw new InvalidOperationException("User already exists");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                Profile = new UserProfile
                {
                    Status = UserStatus.Online,
                    AvatarUrl = "/Assets/avatar_placeholder.png",
                    Bio = "New user"
                }
            };

            _userDirectory[email] = newUser;
            _userPasswords[email] = password;
        }

        public async Task RecoverPasswordAsync(string email)
        {
            var token = await _resetService.GenerateResetTokenAsync(email);
            await _resetService.SendResetLinkAsync(email, token);
        }

        public Task<bool> ChangeEmailAsync(string newEmail)
        {
            if (_currentUser == null || string.IsNullOrWhiteSpace(newEmail)) return Task.FromResult(false);
            if (_userDirectory.ContainsKey(newEmail)) return Task.FromResult(false);

            _userDirectory.Remove(_currentUser.Email);
            _userPasswords[newEmail] = _userPasswords[_currentUser.Email];
            _userPasswords.Remove(_currentUser.Email);

            _currentUser.Email = newEmail;
            _userDirectory[newEmail] = _currentUser;
            return Task.FromResult(true);
        }

        public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (_currentUser == null || string.IsNullOrWhiteSpace(newPassword)) return Task.FromResult(false);
            if (!_userPasswords.TryGetValue(_currentUser.Email, out var stored) || stored != currentPassword) return Task.FromResult(false);

            _userPasswords[_currentUser.Email] = newPassword;
            return Task.FromResult(true);
        }

        public Task UpdateProfileAsync(UserProfile profile)
        {
            if (_currentUser == null || profile == null) return Task.CompletedTask;
            _currentUser.Profile = profile;
            return Task.CompletedTask;
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}
