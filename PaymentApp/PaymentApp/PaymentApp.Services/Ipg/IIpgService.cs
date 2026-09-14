using PaymentApp.Services.Ipg.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApp.Services.Ipg
{
    public interface IIpgService
    {
        Task<string> GetTokenAsync();
        Task<IpgPreparePaymentResponse> PreparePaymentAsync(IpgPreparePaymentRequest request, string apiToken);
        Task<CbiResponse> GetCbiAsync(CbiRequest request, string apiToken);
    }
}
