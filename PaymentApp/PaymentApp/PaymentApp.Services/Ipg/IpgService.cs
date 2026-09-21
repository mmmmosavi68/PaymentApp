using Microsoft.Extensions.Options;
using PaymentApp.Common;
using PaymentApp.Services.Ipg.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentApp.Services.Ipg
{
    public class IpgService : IIpgService, IScopedDependency
    {
        private readonly HttpClient _httpClient;
        private readonly IpgSettings _settings;

        public IpgService(HttpClient httpClient, IOptions<SiteSettings> settings)
        {
            var ipg = settings.Value.IpgSettings;
            _settings = ipg;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ipg.BaseUrl);
        }

        public async Task<string> GetTokenAsync()
        {
            var form = new Dictionary<string, string>
            {
                ["username"] = _settings.Username,
                ["password"] = _settings.Password
            };
            using var content = new FormUrlEncodedContent(form);
            var response = await _httpClient.PostAsync("/cbi/api/token/", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IpgTokenResponse>();
            if (result is null || result.Rescode != 100 || string.IsNullOrEmpty(result.Token))
                throw new InvalidOperationException($"دریافت توکن ناموفق. rescode={result?.Rescode}, msg={result?.Msg}");

            return result.Token;
        }

        public async Task<CbiResponse> GetCbiAsync(CbiRequest request, string apiToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/cbi/api/request/");
            httpRequest.Headers.TryAddWithoutValidation("X_Authorization", $"Token {apiToken}");
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            var rawBody = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<CbiResponse>(
                rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null || result.Rescode != 100 || string.IsNullOrEmpty(result.Cbi))
                throw new InvalidOperationException(
                    $"دریافت شناسه واریز ناموفق. rescode={result?.Rescode}, msg={result?.Msg}, raw={rawBody}");

            return result;
        }

        public async Task<IpgPreparePaymentResponse> PreparePaymentAsync(IpgPreparePaymentRequest request, string apiToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/cashsystem/payment/ipg/preapare/");

            // نکته: برخلاف مستند (که Authorization معمولی نوشته)، سرور واقعی X_Authorization می‌خواد
            httpRequest.Headers.TryAddWithoutValidation("X_Authorization", $"Token {apiToken}");

            var wrapped = new { data = request };
            var json = JsonSerializer.Serialize(wrapped);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var rawBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(
                    $"خطا در سرویس پرداخت. Status={(int)response.StatusCode} {response.StatusCode}, Response={rawBody}");

            var result = JsonSerializer.Deserialize<IpgPreparePaymentResponse>(
                rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                throw new InvalidOperationException($"پاسخ سرویس پرداخت قابل پردازش نیست. raw={rawBody}");

            if (!result.IsTokenCreated)
                throw new InvalidOperationException($"تولید لینک پرداخت ناموفق. msg={result.Msg}, raw={rawBody}");

            return result;
        }
    }
}