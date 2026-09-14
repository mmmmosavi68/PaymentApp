using PaymentApp.Common.MyAttribute;
using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentApp.Web.Areas.Group.ViewModels.Groups
{
    public class GroupCreateViewModel
    {
        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [PersianEnglishInputValidator(ErrorMessage = "لطفاً فقط از حروف فارسی، انگلیسی، اعداد و کاراکترهای مجاز استفاده کنید.")]
        [Display(Name = "نام گروه")]
        public string GroupName { get; set; }
        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }
    }
}
