using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ChatApp.Core;
using ChatApp.Models;
using ChatApp.Services;

namespace ChatApp.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private readonly MainViewModel _mainViewModel;
        private Contact _selectedContact;
        private string _newMessageText;
        private Chat _currentChat;

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

        public ICommand SendCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public ChatViewModel(IChatService chatService, INotificationService notificationService, MainViewModel mainViewModel)
        {
            _chatService = chatService;
            _notificationService = notificationService;
            _mainViewModel = mainViewModel;
            
            SendCommand = new RelayCommand(async o => await SendMessage());
            NavigateProfileCommand = _mainViewModel.NavigateProfileCommand;
            NavigateSettingsCommand = _mainViewModel.NavigateSettingsCommand;

            _chatService.MessageReceived += OnMessageReceived;

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
                SenderId = "me", // In real app, from AuthService.CurrentUser
                Text = NewMessageText,
                IsIncoming = false,
                Status = MessageStatus.Sent
            };

            Messages.Add(msg);
            NewMessageText = "";
            await _chatService.SendMessageAsync(msg);
        }

        private void OnMessageReceived(Message msg)
        {
            Messages.Add(msg);
            
            // Smart Notification Logic:
            // If the chat is with the sender, we might not want to notify if window is active (simulated check)
            bool isActive = true; // Use a real WindowState check in production
            
            // For now, always notify unless we are typing (mock logic) or if it's from current chat
            if (_currentChat == null || _currentChat.ContactId != msg.SenderId)
            {
                 // different contact
                 _notificationService.ShowNotification("New Message", $"Message from {msg.SenderId}");
            }
            else 
            {
                 // same contact, assume active
            }
        }
    }
}
