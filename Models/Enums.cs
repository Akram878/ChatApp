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

    public enum NotificationMode
    {
        Sound,
        Banner,
        Disabled
    }

    public class NotificationSettings
    {
        public NotificationMode Mode { get; set; } = NotificationMode.Banner;
        public bool IsSoundEnabled => Mode == NotificationMode.Sound || Mode == NotificationMode.Banner;
        public bool IsBannerEnabled => Mode == NotificationMode.Banner;
    }
}
