using Microsoft.AspNetCore.Mvc;

namespace PayWithFawry.Controllers
{
    public class WhatsAppSenderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
