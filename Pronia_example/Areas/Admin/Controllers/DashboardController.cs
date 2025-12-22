using Microsoft.AspNetCore.Mvc;

namespace Pronia_example.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View ();
        }
    }
}
