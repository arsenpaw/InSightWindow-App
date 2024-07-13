using AXProductApp.Models.Dto;


namespace AXProductApp.Models
{
    public class Window : DeviceDto
    {

        public bool IsOpen { get; set; }

        public int isAlarm { get; set; }

        public bool IsProtected { get; set; }


    }
}
