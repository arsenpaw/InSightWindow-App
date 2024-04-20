using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Services
{
    public  class NotificationService
    {
        public  void TestSms()
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
        public  void sendAlarmMessage()
        {
            var request = new NotificationRequest
            {
                NotificationId = 100,
                Subtitle = "Alarm",
                Title = "Intrusion Alert",
                Description = "The unexpected intrusion has been detected",
                BadgeNumber = 1,
                Sound = DeviceInfo.Platform == DevicePlatform.Android ? "alarm_sound" : "alarm_sound",
                Android = new AndroidOptions
                {
                    ChannelId = "alarm_sound1"

                },

            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}
