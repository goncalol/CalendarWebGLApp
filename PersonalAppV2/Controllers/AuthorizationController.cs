using Microsoft.AspNetCore.Mvc;

namespace PersonalAppV2.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
