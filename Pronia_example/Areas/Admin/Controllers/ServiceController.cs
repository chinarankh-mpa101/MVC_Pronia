using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_example.Contexts;
using Pronia_example.Models;

namespace Pronia_example.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController(AppDbContext _context) : Controller
    {
        //private readonly AppDbContext _context;

        //public ServiceController(AppDbContext context)
        //{
        //    _context = context;
        //}

        public async Task<IActionResult> Index()
        {
            var features= await _context.AppFeatures.ToListAsync();

            return View(features);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(AppFeature feature)
		{
           if(!ModelState.IsValid)
            {
                return View();
            }
          
            await _context.AppFeatures.AddAsync(feature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
			//return View();
		}

        public async Task <IActionResult> Delete(int id)
		{
            var feature = await  _context.AppFeatures.FindAsync(id);
            if(feature is null)
                return NotFound();
            _context.AppFeatures.Remove(feature);
            await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        // Service-i men evvelden AppFeature modeli kimi qoymusam adini amma casdim controllerde Service controller qoydum adini
        // AppFeature daha uygun ad idi deye ona gore qoymusdum, service mene mentiqsiz gelirdi
    }
}
