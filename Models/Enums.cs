namespace ChatApp.Models
{
    public enum UserStatus
    {
        Online,
        Offline,
        DoNotDisturb
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }

    public class NotificationSettings
    {
        public bool IsSoundEnabled { get; set; } = true;
        public bool IsBannerEnabled { get; set; } = true;
        public bool IsDisabled { get; set; } = false;
    }
}
