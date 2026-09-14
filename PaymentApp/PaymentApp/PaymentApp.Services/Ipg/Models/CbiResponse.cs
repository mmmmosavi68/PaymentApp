using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PaymentApp.Services.Ipg.Models
{
    public class CbiResponse
    {
        [JsonPropertyName("rescode")] public int Rescode { get; set; }
        [JsonPropertyName("msg")] public string? Msg { get; set; }
        [JsonPropertyName("cbi")] public string? Cbi { get; set; }
        [JsonPropertyName("accountnumber")] public string? AccountNumber { get; set; }
        [JsonPropertyName("sheba")] public string? Sheba { get; set; }
        [JsonPropertyName("register_status")] public int? RegisterStatus { get; set; }
        [JsonPropertyName("msg_register")] public string? MsgRegister { get; set; }
    }
}
