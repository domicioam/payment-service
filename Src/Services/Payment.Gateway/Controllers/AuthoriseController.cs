using Microsoft.AspNetCore.Mvc;

namespace Payment.Gateway.Controllers
{
    public class AuthoriseController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}