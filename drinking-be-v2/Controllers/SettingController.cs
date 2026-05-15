using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
