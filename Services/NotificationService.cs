using System;
using System.Diagnostics;

namespace ChatApp.Services
{
    public class NotificationService : INotificationService
    {
        public void ShowNotification(string title, string message)
        {
            // In a real app, this would use Toast Notifications or System Tray
            // Simulating log output
            Debug.WriteLine($"[NOTIFICATION] {title}: {message}");
        }
    }
}
