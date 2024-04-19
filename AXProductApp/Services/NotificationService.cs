using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Services
{
    public static class NotificationService
    {
        public static void TestSms()
        {
            var request = new NotificationRequest
            {
                NotificationId = 1337,
                Title = "Test",
                Subtitle = "New data collected",
                Description = "Local Push Notification",
                BadgeNumber = 1,

                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(3),
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}
