using System.Collections.Generic;
using System;

namespace PaymentApp.Web.Areas.User.ViewModels.UserField
{
    public class UserFieldListViewModel
    {
        public Guid UserId { get; set; }
        public string UserDisplayName { get; set; } // اختیاری برای نشان دادن نام کاربر
        public List<UserFieldViewModel> Fields { get; set; } = new List<UserFieldViewModel>();
    }
}
