using System;
using System.Collections.ObjectModel;

namespace ChatApp.Models
{
    public abstract class Message
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageStatus Status { get; set; }
        public bool IsIncoming { get; set; }

        protected Message()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
            Status = MessageStatus.Sent;
        }
    }

    public class TextMessage : Message
    {
        public string Text { get; set; }
    }

    public class ImageMessage : Message
    {
        public string ImageUrl { get; set; }
    }

    public class DocumentMessage : Message
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }

    public class Chat
    {
        public string ContactId { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
    }
}
