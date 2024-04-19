using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Data
{
    public static class LinkToHub
    {
        public static readonly Uri ArsenTest = new Uri("https://localhost:44324/client-hub");
        public static readonly Uri YuraTest = new Uri("https://localhost:7009/client-hub");
        public static readonly Uri RomaTest = new Uri("https://localhost:7009/client-hub");
        public static readonly Uri RealeseUrl = new Uri("http://192.168.4.2:81/client-hub");
        public static readonly Uri ArsenTestInput = new Uri("https://localhost:44324/user-input-hub");
        public static readonly Uri RealeseUrlInput = new Uri("http://192.168.4.2:81/user-input-hub");
        public static readonly Uri WIFI = new Uri("http://192.168.0.180:81/client-hub");
        public static readonly Uri WIFI_INPUT = new Uri("http://192.168.0.180:81/user-input-hub");
    }
}

