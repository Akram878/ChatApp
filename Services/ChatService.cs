using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatApp.Models;
using System.Linq;
namespace ChatApp.Services
{
    public class ChatService : IChatService
    {

        private readonly Dictionary<string, Chat> _chats = new();
        private readonly List<Contact> _contacts = new();

        public event Action<Message> MessageReceived;
        public event Action<Message> MessageStatusChanged;
        public event Action<Message> MessageEdited;
        public event Action<string> MessageDeleted;

        public ChatService()
        {
            _contacts.AddRange(new[]
            {
                new Contact { Id = "1", Username = "Alice", Status = UserStatus.Online, LastMessage = "Hi there!", AvatarUrl = "/Assets/avatar_placeholder.png" },
                new Contact { Id = "2", Username = "Bob", Status = UserStatus.DoNotDisturb, LastMessage = "See you later.", AvatarUrl = "/Assets/avatar_placeholder.png" },
                new Contact { Id = "3", Username = "Charlie", Status = UserStatus.Offline, LastMessage = "Files attached.", AvatarUrl = "/Assets/avatar_placeholder.png" }
            });

            foreach (var contact in _contacts)
            {
                _chats[contact.Id] = SeedChat(contact.Id);
            }
        }
        public async Task<List<Contact>> GetContactsAsync()
        {
            await Task.Delay(200);
            


            return _contacts;
        }

        public async Task<Chat> GetChatSessionAsync(string contactId)
        {
            await Task.Delay(100);
          




            if (_chats.TryGetValue(contactId, out var chat))
            {
                return chat;
            }

            var newChat = SeedChat(contactId);
            _chats[contactId] = newChat;
            return newChat;
        }

        public async Task SendMessageAsync(string contactId, Message message)
        {
            if (!_chats.TryGetValue(contactId, out var chat))
            {
                chat = SeedChat(contactId);
                _chats[contactId] = chat;
            }

            chat.Messages.Add(message);
            message.Status = MessageStatus.Sent;

            UpdateLastMessage(contactId, message);

            await Task.Delay(300);
            message.Status = MessageStatus.Delivered;
            MessageStatusChanged?.Invoke(message);

            _ = Task.Delay(1000).ContinueWith(_ =>
            {
                message.Status = MessageStatus.Read;
                MessageStatusChanged?.Invoke(message);
            });

            // Simulate echo reply
            _ = Task.Delay(1200).ContinueWith(_ =>
            {
              



                var reply = new TextMessage
                {
                    SenderId = contactId,
                    Text = $"Auto-reply to '{(message as TextMessage)?.Text}'",
                    IsIncoming = true
                };
                _chats[contactId].Messages.Add(reply);
                UpdateLastMessage(contactId, reply);
                MessageReceived?.Invoke(reply);
            });
        }



        public async Task EditMessageAsync(string contactId, string messageId, string newContent)
        {
            await Task.Delay(100);
            if (!_chats.TryGetValue(contactId, out var chat)) return;
            var message = chat.Messages.FirstOrDefault(m => m.Id == messageId) as TextMessage;
            if (message == null) return;
            message.Text = newContent;
            MessageEdited?.Invoke(message);
        }

        public async Task DeleteMessageAsync(string contactId, string messageId)
        {
            await Task.Delay(50);
            if (!_chats.TryGetValue(contactId, out var chat)) return;
            var message = chat.Messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null) return;
            chat.Messages.Remove(message);
            MessageDeleted?.Invoke(messageId);
        }

        private Chat SeedChat(string contactId)
        {
            var chat = new Chat { ContactId = contactId };
            chat.Messages.Add(new TextMessage { SenderId = contactId, Text = "Hello!", Timestamp = DateTime.Now.AddMinutes(-15), IsIncoming = true, Status = MessageStatus.Read });
            chat.Messages.Add(new ImageMessage { SenderId = contactId, ImageUrl = "/Assets/sample-image.png", Timestamp = DateTime.Now.AddMinutes(-10), IsIncoming = true, Status = MessageStatus.Read });
            chat.Messages.Add(new DocumentMessage { SenderId = "me", FileName = "Notes.txt", FilePath = "C:/Docs/Notes.txt", FileSize = 1024, Timestamp = DateTime.Now.AddMinutes(-5), IsIncoming = false, Status = MessageStatus.Delivered });
            return chat;
        }

        private void UpdateLastMessage(string contactId, Message message)
        {
            var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null) return;
            contact.LastMessage = message switch
            {
                TextMessage text => text.Text,
                ImageMessage => "[Image]",
                DocumentMessage doc => $"[Doc] {doc.FileName}",
                _ => contact.LastMessage
            };
        }
    }
}
