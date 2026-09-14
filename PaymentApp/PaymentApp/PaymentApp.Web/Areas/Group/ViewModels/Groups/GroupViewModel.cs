using PaymentApp.Web.Areas.User.ViewModels.User;
using PaymentApp.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentApp.Web.Areas.Group.ViewModels.Groups
{
    public class GroupViewModel : BaseDto<GroupViewModel, Entities.Group.Group, Guid>
    {
        [Display(Name = "نام گروه")]
        public string GroupName { get; set; }
        [Display(Name = "ایجاد کننده")]
        public string CreatorUserName { get; set; }
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
        [Display(Name = "وضعیت ( فعال/غیرفعال)")]
        public IsActiveNameType IsActiveName { get; set; }
    }
}
