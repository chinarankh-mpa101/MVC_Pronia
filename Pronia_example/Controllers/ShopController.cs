using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_example.Contexts;

namespace Pronia_example.Controllers
{
	public class ShopController(AppDbContext _context) : Controller
	{
		public async Task <IActionResult> Index()
		{
			var products= await _context.Products.ToListAsync();
			return View(products);
		}
	}
}
