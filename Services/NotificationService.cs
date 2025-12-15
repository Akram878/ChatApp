using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChatApp.Models;
namespace ChatApp.Services
{
    public class NotificationService : INotificationService
    {

        private readonly List<INotificationSubscriber> _subscribers = new();

        public NotificationMode Mode { get; set; } = NotificationMode.Banner;

        public void Register(INotificationSubscriber subscriber)
        {
            if (!_subscribers.Contains(subscriber))
            {
                _subscribers.Add(subscriber);
            }
        }

        public void Unregister(INotificationSubscriber subscriber)
        {
            if (_subscribers.Contains(subscriber))
            {
                _subscribers.Remove(subscriber);
            }
        }

        public void ShowNotification(string title, string message)
        {
            if (Mode == NotificationMode.Disabled) return;

            Debug.WriteLine($"[NOTIFICATION:{Mode}] {title}: {message}");
            foreach (var subscriber in _subscribers)
            {
                subscriber.OnNotification(title, message);
            }
        }
    }
}
