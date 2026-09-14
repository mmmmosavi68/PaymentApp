using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PaymentApp.Services.Ipg.Models
{
    public class CbiRequest
    {
        [JsonPropertyName("unitcode")] public string UnitCode { get; set; } = null!;
        [JsonPropertyName("sourcecode")] public string SourceCode { get; set; } = null!;
        [JsonPropertyName("nationalcode")] public string NationalCode { get; set; } = null!;
        [JsonPropertyName("deposit")] public long Deposit { get; set; }
    }
}
