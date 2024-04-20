namespace AXProductApp.Data
{
    public class WindowStatus
    {

        public int Temparature { get; set; }

        public int Humidity { get; set; }

        public string IsOpen { get; set; }

        public int isRain { get; set; }

        public string IsProtected { get; set; }

        public DateTime TimeNow { get; set; }

        public int isAlarm { get; set; }    

        public string GetLastConnectInfo()
        {
            string ReturnString;
            TimeSpan TimeDiff = DateTime.Now - TimeNow;
            if (TimeDiff.TotalMinutes <= 1)
                ReturnString = "Online";
            else if (TimeDiff.TotalMinutes <= 59 && TimeDiff.TotalMinutes > 1)
                ReturnString = ($"Last connection {TimeDiff.Minutes} minutes ago");
            else if (TimeDiff.TotalHours < 24)
                ReturnString = ($"Last connection {TimeDiff.Hours} hours ago");
            else
                ReturnString = ($"Last connection {TimeDiff.Days} days ago");
            return ReturnString;
        }
    }
}