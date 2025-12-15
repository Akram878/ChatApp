using ChatApp.Services;
using ChatApp.ViewModels;
using System.ComponentModel.Design;
using System.Windows;
using ChatApp.Core;
namespace ChatApp
{
    public partial class App : Application
    {
        private ServiceContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _container = new ServiceContainer();

            // Composition Root / Dependency Injection
            var passwordReset = new PasswordResetService();
            var authService = new AuthService(passwordReset);
            var chatService = new ChatService();
            var notificationService = new NotificationService();
            var googleProvider = new GoogleAuthService();
            var appleProvider = new AppleAuthService();

            _container.RegisterSingleton<IAuthService>(authService);
            _container.RegisterSingleton<IChatService>(chatService);
            _container.RegisterSingleton<INotificationService>(notificationService);
            _container.RegisterSingleton<IExternalAuthProvider>(googleProvider); // default resolution
            _container.RegisterSingleton<GoogleAuthService>(googleProvider);
            _container.RegisterSingleton<AppleAuthService>(appleProvider);

            var mainViewModel = new MainViewModel(authService, chatService, notificationService, googleProvider, appleProvider);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }
}
