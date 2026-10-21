using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentApp.Common;
using PaymentApp.Services.Ipg.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ILogger<IpgService> _logger;

        public IpgService(HttpClient httpClient, IOptions<SiteSettings> settings, ILogger<IpgService> logger)
        {
            var ipg = settings.Value.IpgSettings;
            _settings = ipg;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ipg.BaseUrl);
        }

        public async Task<string> GetTokenAsync()
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("IPG GetToken: sending request. BaseUrl={BaseUrl}", _settings.BaseUrl);

            var form = new Dictionary<string, string>
            {
                ["username"] = _settings.Username,
                ["password"] = _settings.Password
            };
            using var content = new FormUrlEncodedContent(form);

            var response = await _httpClient.PostAsync("/cbi/api/token/", content);
            _logger.LogInformation("IPG GetToken: response received. HttpStatus={HttpStatus}, ElapsedMs={ElapsedMs}",
                (int)response.StatusCode, sw.ElapsedMilliseconds);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IpgTokenResponse>();
            if (result is null || result.Rescode != 100 || string.IsNullOrEmpty(result.Token))
            {
                // بدنه‌ی پاسخ عمداً لاگ نمی‌شه (ممکنه توکن داشته باشه)
                _logger.LogError("IPG GetToken failed. Rescode={Rescode}, Msg={Msg}", result?.Rescode, result?.Msg);
                throw new InvalidOperationException($"دریافت توکن ناموفق. rescode={result?.Rescode}, msg={result?.Msg}");
            }

            return result.Token;
        }

        public async Task<CbiResponse> GetCbiAsync(CbiRequest request, string apiToken)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("IPG GetCbi: sending request. UnitCode={UnitCode}, SourceCode={SourceCode}, Deposit={Deposit}",
                request.UnitCode, request.SourceCode, request.Deposit);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/cbi/api/request/");
            httpRequest.Headers.TryAddWithoutValidation("X_Authorization", $"Token {apiToken}");
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            var rawBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("IPG GetCbi: response received. HttpStatus={HttpStatus}, ElapsedMs={ElapsedMs}",
                (int)response.StatusCode, sw.ElapsedMilliseconds);

            var result = JsonSerializer.Deserialize<CbiResponse>(
                rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null || result.Rescode != 100 || string.IsNullOrEmpty(result.Cbi))
            {
                _logger.LogError("IPG GetCbi failed. Rescode={Rescode}, Msg={Msg}, Raw={Raw}",
                    result?.Rescode, result?.Msg, rawBody);
                throw new InvalidOperationException(
                    $"دریافت شناسه واریز ناموفق. rescode={result?.Rescode}, msg={result?.Msg}, raw={rawBody}");
            }

            return result;
        }

        public async Task<IpgPreparePaymentResponse> PreparePaymentAsync(IpgPreparePaymentRequest request, string apiToken)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("IPG PreparePayment: sending request. BillNumber={BillNumber}, Amount={Amount}, RedirectUrl={RedirectUrl}",
                request.BillNumber, request.Depositparams.Amount, request.RedirectUrl);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/cashsystem/payment/ipg/preapare/");

            // نکته: برخلاف مستند (که Authorization معمولی نوشته)، سرور واقعی X_Authorization می‌خواد
            httpRequest.Headers.TryAddWithoutValidation("X_Authorization", $"Token {apiToken}");

            var wrapped = new { data = request };
            var json = JsonSerializer.Serialize(wrapped);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var rawBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("IPG PreparePayment: response received. HttpStatus={HttpStatus}, ElapsedMs={ElapsedMs}",
                (int)response.StatusCode, sw.ElapsedMilliseconds);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("IPG PreparePayment HTTP error. HttpStatus={HttpStatus}, Raw={Raw}",
                    (int)response.StatusCode, rawBody);
                throw new InvalidOperationException(
                    $"خطا در سرویس پرداخت. Status={(int)response.StatusCode} {response.StatusCode}, Response={rawBody}");
            }

            var result = JsonSerializer.Deserialize<IpgPreparePaymentResponse>(
                rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                _logger.LogError("IPG PreparePayment: unparsable response. Raw={Raw}", rawBody);
                throw new InvalidOperationException($"پاسخ سرویس پرداخت قابل پردازش نیست. raw={rawBody}");
            }

            if (!result.IsTokenCreated)
            {
                _logger.LogError("IPG PreparePayment rejected. Rescode={Rescode}, Msg={Msg}, Raw={Raw}",
                    result.Rescode, result.Msg, rawBody);
                throw new InvalidOperationException($"تولید لینک پرداخت ناموفق. msg={result.Msg}, raw={rawBody}");
            }

            // در حالت موفق، rawBody لاگ نمی‌شه چون شامل توکن SEP هست
            return result;
        }
    }
}