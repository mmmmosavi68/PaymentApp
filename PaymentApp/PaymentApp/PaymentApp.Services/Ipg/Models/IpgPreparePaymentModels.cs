using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PaymentApp.Services.Ipg.Models
{
    public class IpgPreparePaymentRequest
    {
        public string Psp { get; set; } = "SEP";
        public string? Nationalcode { get; set; }
        public string BillNumber { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
        public DepositParams Depositparams { get; set; } = null!;
    }

    public class DepositParams
    {
        public long Amount { get; set; }
        public List<IbanItem> Ibans { get; set; } = new();
    }

    public class IbanItem
    {
        [JsonPropertyName("IBAN")] public string Iban { get; set; } = null!;
        public long Amount { get; set; }
        public string PurchaseID { get; set; } = null!;
    }

    public class IpgPreparePaymentResponse
    {
        public bool IsTokenCreated { get; set; }
        public string? Token { get; set; }
        public string? Payurls { get; set; }
        public int Pay_id { get; set; }
        public string? Msg { get; set; }
        public int? Rescode { get; set; }
    }
}
