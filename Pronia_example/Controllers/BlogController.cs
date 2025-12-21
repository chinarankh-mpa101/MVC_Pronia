using Microsoft.AspNetCore.Mvc;

namespace Pronia_example.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
