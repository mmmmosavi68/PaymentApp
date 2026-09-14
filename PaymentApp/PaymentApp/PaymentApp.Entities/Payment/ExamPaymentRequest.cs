using PaymentApp.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Entities.Payment
{
    public enum PaymentRequestStatus { Pending = 0, Success = 1, Failed = 2 }

    public class ExamPaymentRequest : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string FatherName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string BillNumber { get; set; } = null!;
        public string? SepToken { get; set; }
        public int? IdPay { get; set; }
        public string? PayUrl { get; set; }

        public string? ResNum { get; set; }
        public string? ReferenceCode { get; set; }
        public string? TraceNo { get; set; }
        public string? VerifyDetail { get; set; }

        public PaymentRequestStatus Status { get; set; } = PaymentRequestStatus.Pending;
    }
}
