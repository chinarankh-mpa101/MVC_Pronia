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

           var isExist= await _context.AppFeatures.AnyAsync(f=>f.Title==feature.Title);
            if(isExist)
			{
                ModelState.AddModelError("Title", "Bu title-da service movucuddur artiqqq!");
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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var services= await _context.AppFeatures.FindAsync(id);
            if (services is null) // service is null
                return NotFound();
            return View(services);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppFeature feature)
        {
            if (!ModelState.IsValid)
				return View();
            var existFeature= await _context.AppFeatures.FindAsync(feature.Id);
            if (existFeature is null)
				return BadRequest();
            existFeature.Title= feature.Title;
            existFeature.Description= feature.Description;
            existFeature.ImageUrl= feature.ImageUrl;
            _context.AppFeatures.Update(existFeature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Service-i men evvelden AppFeature modeli kimi qoymusam adini amma casdim controllerde Service controller qoydum adini
        // AppFeature daha uygun ad idi deye ona gore qoymusdum, service mene mentiqsiz gelirdi
    }
}
