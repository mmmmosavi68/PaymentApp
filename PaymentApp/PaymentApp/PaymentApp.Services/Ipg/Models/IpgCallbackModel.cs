using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Services.Ipg.Models
{
    public class IpgCallbackModel
    {
        public string? Token { get; set; }
        public string? BillNumber { get; set; }
        public string? Resnum { get; set; }
        public string? Referencecode { get; set; }
        public string? TraceNo { get; set; }
        public int Status { get; set; }
        public string? VerifyDetail { get; set; }
    }
}
