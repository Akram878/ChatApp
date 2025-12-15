using System;

namespace ChatApp.Models
{
    public class UserProfile
    {
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public UserStatus Status { get; set; }
        public NotificationSettings NotificationSettings { get; set; } = new NotificationSettings();
    }

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserProfile Profile { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            Profile = new UserProfile();
        }
    }

    public class Contact
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public string LastMessage { get; set; }
    }
}
