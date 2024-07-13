namespace AXProductApp.Models
{
    public class WindowStatus
    {

        public int Temparature { get; set; }

        public int Humidity { get; set; }

        public bool IsOpen { get; set; }

        public int isRain { get; set; }

        public bool IsProtected { get; set; }

        public DateTime TimeNow { get; set; }

        public int isAlarm { get; set; }

       
    }
}