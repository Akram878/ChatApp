using System.Windows;
using ChatApp.Services;
using ChatApp.ViewModels;

namespace ChatApp
{
    public partial class App : Application
    {
        private IAuthService _authService;
        private IChatService _chatService;
        private INotificationService _notificationService;
        private MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Composition Root / Dependency Injection
            _authService = new AuthService();
            _chatService = new ChatService();
            _notificationService = new NotificationService();

            _mainViewModel = new MainViewModel(_authService, _chatService, _notificationService);

            var mainWindow = new MainWindow
            {
                DataContext = _mainViewModel
            };

            mainWindow.Show();
        }
    }
}
