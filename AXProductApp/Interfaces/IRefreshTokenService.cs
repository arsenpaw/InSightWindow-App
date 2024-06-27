using AXProductApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Interfaces
{
     interface IRefreshTokenService
    {
        Task<HttpStatusCode> UpdateTokens();
    }
}
