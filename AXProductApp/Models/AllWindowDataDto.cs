using AXProductApp.Models;


namespace AXProductApp.Models
{
    public class AllWindowDataDto: Window
    {
        public int Temparature { get; set; }

        public int Humidity { get; set; }

        public bool isRain { get; set; }

        public DateTime TimeNow { get; set; }


        public string GetLastConnectInfo()
        {
            string ReturnString;
            TimeSpan TimeDiff = DateTime.Now - TimeNow;
            if (TimeDiff.TotalMinutes <= 1)
                ReturnString = "Online";
            else if (TimeDiff.TotalMinutes <= 59 && TimeDiff.TotalMinutes > 1)
                ReturnString = $"Last connection {TimeDiff.Minutes} minutes ago";
            else if (TimeDiff.TotalHours < 24)
                ReturnString = $"Last connection {TimeDiff.Hours} hours ago";
            else
                ReturnString = $"Last connection {TimeDiff.Days} days ago";
            return ReturnString;
        }

    }
}
