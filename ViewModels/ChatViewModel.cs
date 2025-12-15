using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Models;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class ChatViewModel : ViewModelBase, INotificationSubscriber
    {

        private readonly IAuthService _authService;
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private readonly MainViewModel _mainViewModel;
        private Contact _selectedContact;
        private string _newMessageText;
        private Chat _currentChat;
        private Message _selectedMessage;
        private bool _isUserActive = true;

        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                if (_selectedContact != null)
                {
                    LoadChat(_selectedContact.Id);
                }
            }
        }

        public string NewMessageText
        {
            get => _newMessageText;
            set { _newMessageText = value; OnPropertyChanged(); }
        }

        public Message SelectedMessage
        {
            get => _selectedMessage;
            set { _selectedMessage = value; OnPropertyChanged(); }
        }


        public ICommand SendCommand { get; }
        public ICommand SendImageCommand { get; }
        public ICommand SendDocumentCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand EditMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }
        public ChatViewModel(IAuthService authService, IChatService chatService, INotificationService notificationService, MainViewModel mainViewModel)
        {
            _authService = authService;
            _chatService = chatService;
            _notificationService = notificationService;
            _mainViewModel = mainViewModel;
            
            SendCommand = new RelayCommand(async o => await SendMessage());

            SendImageCommand = new RelayCommand(async o => await SendImage());
            SendDocumentCommand = new RelayCommand(async o => await SendDocument());
            NavigateProfileCommand = _mainViewModel.NavigateProfileCommand;
            NavigateSettingsCommand = _mainViewModel.NavigateSettingsCommand;
            EditMessageCommand = new RelayCommand(async o => await EditSelectedMessage(o as Message));
            DeleteMessageCommand = new RelayCommand(async o => await DeleteSelectedMessage(o as Message));
            _chatService.MessageReceived += OnMessageReceived;
            _chatService.MessageStatusChanged += OnMessageStatusChanged;
            _chatService.MessageEdited += OnMessageEdited;
            _chatService.MessageDeleted += OnMessageDeleted;
            _notificationService.Register(this);
            LoadContacts();
        }

        private async void LoadContacts()
        {
            var contacts = await _chatService.GetContactsAsync();
            Contacts.Clear();
            foreach (var c in contacts) Contacts.Add(c);
        }

        private async void LoadChat(string contactId)
        {
            _currentChat = await _chatService.GetChatSessionAsync(contactId);
            Messages.Clear();
            foreach (var m in _currentChat.Messages) Messages.Add(m);
        }

        private async System.Threading.Tasks.Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText) || SelectedContact == null) return;

            var msg = new TextMessage
            {
                SenderId = _authService.CurrentUser?.Id ?? "me",
                Text = NewMessageText,
                IsIncoming = false,
                Status = MessageStatus.Sent
            };

            Messages.Add(msg);
            NewMessageText = "";
            await _chatService.SendMessageAsync(SelectedContact.Id, msg);
        }

        private async System.Threading.Tasks.Task SendImage()
        {
            if (SelectedContact == null) return;
            var msg = new ImageMessage
            {
                SenderId = _authService.CurrentUser?.Id ?? "me",
                ImageUrl = "/Assets/sample-image.png",
                IsIncoming = false,
                Status = MessageStatus.Sent
            };
            Messages.Add(msg);
            await _chatService.SendMessageAsync(SelectedContact.Id, msg);
        }

        private async System.Threading.Tasks.Task SendDocument()
        {
            if (SelectedContact == null) return;
            var msg = new DocumentMessage
            {
                SenderId = _authService.CurrentUser?.Id ?? "me",
                FileName = "Document.pdf",
                FilePath = "C:/Docs/Document.pdf",
                FileSize = 2048,
                IsIncoming = false,
                Status = MessageStatus.Sent
            };
            Messages.Add(msg);
            await _chatService.SendMessageAsync(SelectedContact.Id, msg);
        }

        private async System.Threading.Tasks.Task EditSelectedMessage(Message message)
        {
            if (message is not TextMessage textMessage || SelectedContact == null) return;
            await _chatService.EditMessageAsync(SelectedContact.Id, textMessage.Id, textMessage.Text + " (edited)");
        }

        private async System.Threading.Tasks.Task DeleteSelectedMessage(Message message)
        {
            if (message == null || SelectedContact == null) return;
            await _chatService.DeleteMessageAsync(SelectedContact.Id, message.Id);
        }

        private void OnMessageReceived(Message msg)
        {
            Messages.Add(msg);


            if (_currentChat == null || _currentChat.ContactId != msg.SenderId || !_isUserActive)
            {
               
                 _notificationService.ShowNotification("New Message", $"Message from {msg.SenderId}");
            }
        }

        private void OnMessageStatusChanged(Message message)
        {
            OnPropertyChanged(nameof(Messages));
        }

        private void OnMessageEdited(Message message)
        {
            var existing = Messages.FirstOrDefault(m => m.Id == message.Id) as TextMessage;
            if (existing != null)
            {
                existing.Text = ((TextMessage)message).Text;
                OnPropertyChanged(nameof(Messages));
            }
        }

        private void OnMessageDeleted(string messageId)
        {
            var existing = Messages.FirstOrDefault(m => m.Id == messageId);
            if (existing != null)
            {
                Messages.Remove(existing);
            }
        }

        public void OnNotification(string title, string message)
        {
            // placeholder for UI reaction to notifications
        }
    }
}
