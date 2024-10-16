﻿using AXProductApp.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Interfaces
{
    interface IMainMenu
    {
        Task OnAppUpdateAsync();
        Task<List<DeviceDto>> GetUserDevicesListAsync();
    }
}
