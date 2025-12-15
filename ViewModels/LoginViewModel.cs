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
        private readonly IExternalAuthProvider _googleProvider;
        private readonly IExternalAuthProvider _appleProvider;
        private string _email;
        private string _password;
        private string _errorMessage;
        private string _recoveryStatus;

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
        public string RecoveryStatus
        {
            get => _recoveryStatus;
            set { _recoveryStatus = value; OnPropertyChanged(); }
        }


        public ICommand LoginCommand { get; }
        public ICommand GoogleLoginCommand { get; }
        public ICommand AppleLoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public LoginViewModel(IAuthService authService, MainViewModel mainViewModel, IExternalAuthProvider googleProvider, IExternalAuthProvider appleProvider)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;
            _googleProvider = googleProvider;
            _appleProvider = appleProvider;
            LoginCommand = new RelayCommand(async o => await Login());
            GoogleLoginCommand = new RelayCommand(async o => await ExternalLogin(_googleProvider));
            AppleLoginCommand = new RelayCommand(async o => await ExternalLogin(_appleProvider));
            GoToRegisterCommand = _mainViewModel.NavigateRegisterCommand;
            RecoverPasswordCommand = new RelayCommand(async o => await RecoverPassword());
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

        private async Task ExternalLogin(IExternalAuthProvider provider)
        {
            if (await _authService.LoginWithProviderAsync(provider))
            {
                _mainViewModel.NavigateChatCommand.Execute(null);

            }
            else
            {
                ErrorMessage = $"{provider.ProviderName} login failed";
            }
        }

        private async Task RecoverPassword()
        {
            await _authService.RecoverPasswordAsync(Email);
            RecoveryStatus = "Recovery email sent";
        }
    }
}
