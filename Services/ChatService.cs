using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        public event Action<Message> MessageReceived;

        public async Task<List<Contact>> GetContactsAsync()
        {
            await Task.Delay(200);
            return new List<Contact>
            {
                new Contact { Id = "1", Username = "Alice", Status = UserStatus.Online, LastMessage = "Hi there!" },
                new Contact { Id = "2", Username = "Bob", Status = UserStatus.DoNotDisturb, LastMessage = "See you later." },
                new Contact { Id = "3", Username = "Charlie", Status = UserStatus.Offline, LastMessage = "Files attached." }
            };
        }

        public async Task<Chat> GetChatSessionAsync(string contactId)
        {
            await Task.Delay(100);
            var chat = new Chat { ContactId = contactId };
            // Simulate existing messages
            chat.Messages.Add(new TextMessage { SenderId = contactId, Text = "Hello!", Timestamp = DateTime.Now.AddMinutes(-5), IsIncoming = true });
            return chat;
        }

        public async Task SendMessageAsync(Message message)
        {
            await Task.Delay(300); // Simulate send
            message.Status = MessageStatus.Sent;
            
            // Validate simulation of read receipt
            _ = Task.Delay(1000).ContinueWith(t => 
            {
                message.Status = MessageStatus.Delivered;
            });

            // Simulate echo reply
            _ = Task.Delay(2000).ContinueWith(t => 
            {
               MessageReceived?.Invoke(new TextMessage 
               { 
                   SenderId = "echo", 
                   Text = "Echo: " + (message as TextMessage)?.Text, 
                   IsIncoming = true 
               });
            });
        }
    }
}
