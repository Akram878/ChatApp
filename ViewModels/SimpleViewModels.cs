using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Models;
using ChatApp.Services;
using System;
namespace ChatApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly MainViewModel _mainViewModel;
        private User _user;
        private string _statusMessage;

        public Array StatusOptions => Enum.GetValues(typeof(UserStatus));
        public User User
        {
            get => _user;
            set { _user = value; OnPropertyChanged(); }
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand BackCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ProfileViewModel(IAuthService authService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;
            User = _authService.CurrentUser ?? new User { Username = "Guest", Email = "guest@example.com" }; // Fallback
            BackCommand = _mainViewModel.NavigateChatCommand;
            SaveProfileCommand = new RelayCommand(async o => await SaveAsync());
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            await _authService.UpdateProfileAsync(User.Profile);
            StatusMessage = "Profile updated";
        }
    }

    public class SettingsViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private string _newEmail;
        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;
        private NotificationMode _notificationMode;
        private string _status;
        public bool IsDarkTheme { get; set; } = true; // Simulated
        public string NewEmail { get => _newEmail; set { _newEmail = value; OnPropertyChanged(); } }
        public string CurrentPassword { get => _currentPassword; set { _currentPassword = value; OnPropertyChanged(); } }
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }
        public NotificationMode NotificationMode { get => _notificationMode; set { _notificationMode = value; OnPropertyChanged(); } }
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }
        public Array NotificationModes => Enum.GetValues(typeof(NotificationMode));
        public ICommand BackCommand { get; }
        public ICommand ChangeEmailCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand UpdateNotificationCommand { get; }
        public SettingsViewModel(IAuthService authService, INotificationService notificationService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _notificationService = notificationService;
            _mainViewModel = mainViewModel;
            NotificationMode = _authService.CurrentUser?.Profile?.NotificationSettings?.Mode ?? NotificationMode.Banner;
            BackCommand = _mainViewModel.NavigateChatCommand;
            ChangeEmailCommand = new RelayCommand(async o => await ChangeEmail());
            ChangePasswordCommand = new RelayCommand(async o => await ChangePassword());
            UpdateNotificationCommand = new RelayCommand(o => UpdateNotification());
        }

        private async System.Threading.Tasks.Task ChangeEmail()
        {
            var success = await _authService.ChangeEmailAsync(NewEmail);
            Status = success ? "Email updated" : "Email update failed";
        }

        private async System.Threading.Tasks.Task ChangePassword()
        {
            if (NewPassword != ConfirmPassword)
            {
                Status = "Passwords do not match";
                return;
            }

            var success = await _authService.ChangePasswordAsync(CurrentPassword, NewPassword);
            Status = success ? "Password updated" : "Password update failed";
        }

        private void UpdateNotification()
        {
            if (_authService.CurrentUser?.Profile?.NotificationSettings != null)
            {
                _authService.CurrentUser.Profile.NotificationSettings.Mode = NotificationMode;
            }
            _notificationService.Mode = NotificationMode;
            Status = "Notification settings saved";
        }
    }
}
