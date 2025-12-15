using System.Collections.Generic;
using System.Threading.Tasks;
using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IAuthService
    {
        User CurrentUser { get; }
        Task<bool> LoginAsync(string email, string password);
        Task<bool> LoginWithGoogleAsync();
        Task<bool> LoginWithAppleAsync();
        Task RegisterAsync(string email, string password, string username);
        Task RecoverPasswordAsync(string email);
        void Logout();
    }

    public interface IChatService
    {
        Task<List<Contact>> GetContactsAsync();
        Task<Chat> GetChatSessionAsync(string contactId);
        Task SendMessageAsync(Message message);
        event System.Action<Message> MessageReceived;
    }

    public interface INotificationService
    {
        void ShowNotification(string title, string message);
    }
}
