using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly MainViewModel _mainViewModel;
        private string _email;
        private string _password;
        private string _errorMessage;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoogleLoginCommand { get; }
        public ICommand AppleLoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(IAuthService authService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;

            LoginCommand = new RelayCommand(async o => await Login());
            GoogleLoginCommand = new RelayCommand(async o => await GoogleLogin());
            AppleLoginCommand = new RelayCommand(async o => await AppleLogin());
            GoToRegisterCommand = _mainViewModel.NavigateRegisterCommand;
        }

        private async Task Login()
        {
            var success = await _authService.LoginAsync(Email, Password);
            if (success)
            {
                _mainViewModel.NavigateChatCommand.Execute(null);
            }
            else
            {
                ErrorMessage = "Invalid credentials";
            }
        }

        private async Task GoogleLogin()
        {
            if (await _authService.LoginWithGoogleAsync())
                _mainViewModel.NavigateChatCommand.Execute(null);
        }

        private async Task AppleLogin()
        {
            if (await _authService.LoginWithAppleAsync())
                _mainViewModel.NavigateChatCommand.Execute(null);
        }
    }
}
