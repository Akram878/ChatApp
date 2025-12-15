using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly MainViewModel _mainViewModel;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _username;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel(IAuthService authService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;

            RegisterCommand = new RelayCommand(async o => 
            {
                if (Password == ConfirmPassword)
                {
                    await _authService.RegisterAsync(Email, Password, Username);
                    _mainViewModel.NavigateLoginCommand.Execute(null);
                }
            });
            BackToLoginCommand = _mainViewModel.NavigateLoginCommand;
        }
    }
}
