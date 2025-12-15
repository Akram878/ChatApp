using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Models;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly MainViewModel _mainViewModel;
        private User _user;

        public User User
        {
            get => _user;
            set { _user = value; OnPropertyChanged(); }
        }

        public ICommand BackCommand { get; }

        public ProfileViewModel(IAuthService authService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;
            User = _authService.CurrentUser ?? new User { Username = "Guest", Email = "guest@example.com" }; // Fallback
            BackCommand = _mainViewModel.NavigateChatCommand;
        }
    }

    public class SettingsViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        
        public bool IsDarkTheme { get; set; } = true; // Simulated

        public ICommand BackCommand { get; }

        public SettingsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            BackCommand = _mainViewModel.NavigateChatCommand;
        }
    }
}
