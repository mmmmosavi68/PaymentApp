
using System.Collections.Generic;

namespace PaymentApp.Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public AuthenticationSettings authenticationSettings { get; set; }
        public AuthenticationStudentSettings authenticationStudentSettings { get; set; }
        public PayamSMS payamSMS { get; set; }
        public FileSettings FileSettings { get; set; }
        public bool IpFiltering { get; set; }
        public string[] AllowedIpRanges { get; set; }
        public string RedirectURL { get; set; }

        public IpgSettings IpgSettings { get; set; }   // <-- این خط اضافه شد

    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }

    }

    public class AuthenticationSettings
    {
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public int ExpireTimeSpanInMinutes { get; set; }
    }
    public class AuthenticationStudentSettings
    {
        public string StudentLoginPath { get; set; }
        public string StudentLogoutPath { get; set; }
        public int StudentExpireTimeSpanInMinutes { get; set; }
        public int MaxSmsPerDay { get; set; }
    }

    public class PayamSMS
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ClientConfig
    {
        public string BaseUrl { get; set; }
        public string ApiPrefix { get; set; }
        public string Environment { get; set; }
    }

    public class FileSettings
    {
        public List<string> AllowedUploadExtensions { get; set; } = new();
        public int MaxUploadFileSizeInMB { get; set; } = 10; // مقدار پیش‌فرض
    }


    public class IpgSettings
    {
        public string BaseUrl { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
        public string UnitCode { get; set; } = null!;      // <-- جایگزین Iban
        public string SourceCode { get; set; } = null!;    // <-- جایگزین PurchaseId
    }
}