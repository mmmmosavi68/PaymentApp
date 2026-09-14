using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Services.Ipg.Models
{
    public class IpgTokenResponse
    {
        public int Rescode { get; set; }
        public string? Msg { get; set; }
        public string? Token { get; set; }
    }
}
