using System;
using System.Collections.Generic;

namespace PaymentApp.Web.Areas.User.ViewModels.Menu
{
    public class ModifyUserAcceesMenuViewModel
    {
        public List<Guid> List { get; set; }
        public Guid UserId { get; set; }
    }
}
