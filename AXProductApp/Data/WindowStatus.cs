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

        public string StringTimeFromLastConnection { get; set; }

}
}