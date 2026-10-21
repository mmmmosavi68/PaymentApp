using PaymentApp.Entities.Payment;
using System.Text.Json.Serialization;

namespace PaymentApp.Web.Areas.Payment.ViewModels
{
    public class PaymentResultViewModel
    {
        public string ExamTitle { get; set; } = null!;
        public long Amount { get; set; }
        public string? ResNum { get; set; }
        public string? ReferenceCode { get; set; }
        public string? PdfUrl { get; set; }
        public PaymentRequestStatus Status { get; set; }
    }
}
