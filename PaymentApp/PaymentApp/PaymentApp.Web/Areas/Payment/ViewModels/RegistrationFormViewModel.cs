using System.ComponentModel.DataAnnotations;

namespace PaymentApp.Web.Areas.Payment.ViewModels
{
    public class RegistrationFormViewModel
    {
        [Required]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "کد ملی الزامی است")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
        public string NationalCode { get; set; } = null!;

        [Required(ErrorMessage = "شماره همراه الزامی است")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره همراه معتبر نیست")]
        public string Mobile { get; set; } = null!;

        [Required(ErrorMessage = "نام پدر الزامی است")]
        public string FatherName { get; set; } = null!;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        public string Email { get; set; } = null!;
    }
}
