using ChatApp.Core;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        private readonly IAuthService _authService;
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;

        // Navigation Commands
        public RelayCommand NavigateLoginCommand { get; }
        public RelayCommand NavigateRegisterCommand { get; }
        public RelayCommand NavigateChatCommand { get; }
        public RelayCommand NavigateProfileCommand { get; }
        public RelayCommand NavigateSettingsCommand { get; }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IAuthService authService, IChatService chatService, INotificationService notificationService)
        {
            _authService = authService;
            _chatService = chatService;
            _notificationService = notificationService;

            NavigateLoginCommand = new RelayCommand(o => CurrentView = new LoginViewModel(_authService, this));
            NavigateRegisterCommand = new RelayCommand(o => CurrentView = new RegisterViewModel(_authService, this));
            NavigateChatCommand = new RelayCommand(o => CurrentView = new ChatViewModel(_chatService, _notificationService, this));
            NavigateProfileCommand = new RelayCommand(o => CurrentView = new ProfileViewModel(_authService, this));
            NavigateSettingsCommand = new RelayCommand(o => CurrentView = new SettingsViewModel(this));

            // Default view
            CurrentView = new LoginViewModel(_authService, this);
        }
    }
}
