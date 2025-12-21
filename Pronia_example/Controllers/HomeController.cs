using Microsoft.AspNetCore.Mvc;
using Pronia_example.Contexts;
using Pronia_example.Models;

namespace Pronia_example.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            //AppDbContext context = new AppDbContext();
            List<AppFeature> features= _context.AppFeatures.ToList();
            //ViewBag.AppFeatures = features;
            return View(features);
        }
    }
}
