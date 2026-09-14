using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaymentApp.Entities.Common;

namespace PaymentApp.Entities.Student
{
    public class StudentOtp : BaseEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public string Code { get; set; } // بهتره هش بشه
        public DateTime ExpiresAt { get; set; }// تاریخ انقضای OTP
        public bool IsUsed { get; set; } = false;
        public string SentTo { get; set; } // شماره موبایل 

    }
}
