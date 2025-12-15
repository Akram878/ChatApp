using System.Collections.Generic;
using System.Threading.Tasks;
using ChatApp.Models;

namespace ChatApp.Services
{



    public interface IExternalAuthProvider
    {
        Task<User> AuthenticateAsync();
        string ProviderName { get; }
    }

    public interface IPasswordResetService
    {
        Task<string> GenerateResetTokenAsync(string email);
        Task<bool> SendResetLinkAsync(string email, string token);
        Task<bool> ValidateResetTokenAsync(string email, string token);
    }

    public interface IAuthService
    {
        User CurrentUser { get; }
        Task<bool> LoginAsync(string email, string password);
        Task<bool> LoginWithProviderAsync(IExternalAuthProvider provider);
        Task RegisterAsync(string email, string password, string username);
        Task RecoverPasswordAsync(string email);


        Task<bool> ChangeEmailAsync(string newEmail);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task UpdateProfileAsync(UserProfile profile);
        void Logout();
    }

    public interface IChatService
    {
        Task<List<Contact>> GetContactsAsync();
        Task<Chat> GetChatSessionAsync(string contactId);
        Task SendMessageAsync(string contactId, Message message);
        Task EditMessageAsync(string contactId, string messageId, string newContent);
        Task DeleteMessageAsync(string contactId, string messageId);
        event System.Action<Message> MessageReceived;
        event System.Action<Message> MessageStatusChanged;
        event System.Action<Message> MessageEdited;
        event System.Action<string> MessageDeleted;
    }

    public interface INotificationSubscriber
    {
        void OnNotification(string title, string message);
    }

    public interface INotificationService
    {
        NotificationMode Mode { get; set; }
        void Register(INotificationSubscriber subscriber);
        void Unregister(INotificationSubscriber subscriber);
        void ShowNotification(string title, string message);
    }
}
