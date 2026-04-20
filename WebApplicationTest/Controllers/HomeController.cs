using Microsoft.AspNetCore.Mvc;

namespace WebApplicationTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}