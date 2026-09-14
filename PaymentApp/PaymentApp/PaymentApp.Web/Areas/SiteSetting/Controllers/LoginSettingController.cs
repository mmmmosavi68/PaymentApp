using Microsoft.AspNetCore.Mvc;

namespace PaymentApp.Web.Areas.SiteSetting.Controllers
{
    public class LoginSettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
