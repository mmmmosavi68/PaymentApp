using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PaymentApp.Services.Ipg.Models
{
    public class IpgPreparePaymentRequest
    {
        [JsonPropertyName("psp")]
        public string Psp { get; set; } = "SEP";

        [JsonPropertyName("nationalcode")]
        public string? Nationalcode { get; set; }

        [JsonPropertyName("billNumber")]
        public string BillNumber { get; set; } = null!;

        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; } = null!;

        [JsonPropertyName("depositparams")]
        public DepositParams Depositparams { get; set; } = null!;
    }

    public class DepositParams
    {
        [JsonPropertyName("Amount")]
        public long Amount { get; set; }

        [JsonPropertyName("ibans")]
        public List<IbanItem> Ibans { get; set; } = new();
    }

    public class IbanItem
    {
        [JsonPropertyName("IBAN")]
        public string Iban { get; set; } = null!;

        [JsonPropertyName("Amount")]
        public long Amount { get; set; }

        [JsonPropertyName("PurchaseID")]
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
