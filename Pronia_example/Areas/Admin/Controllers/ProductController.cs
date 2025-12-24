using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Pronia_example.Contexts;
using Pronia_example.Migrations;
using System.Reflection;


namespace Pronia_example.Areas.Admin.Controllers
{
	[Area("Admin")]

	public class ProductController(AppDbContext _context) : Controller
	{
		public async Task<IActionResult> Index()
		{
			var products = await _context.Products.Include(x => x.Category).ToListAsync();
			return View(products);
		}


		public async Task<IActionResult> Create()
		{
			var categories = await _context.Categories.ToListAsync();
			ViewBag.Categories = categories;

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product product)
		{
			

			if (!ModelState.IsValid)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				return View();
			}
			var isExistCategory= await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
			if (!isExistCategory)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
				return View(product);
			}
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}



		public async Task<IActionResult> Delete(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product is null)
				return NotFound();
			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}


		public async Task<IActionResult> Update(int id)
		{
			var product= await _context.Products.FindAsync(id);
			if(product is null)
				return NotFound();
			var categories = await _context.Categories.ToListAsync();
			ViewBag.Categories = categories;
			return View(product);



		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(Product product)
		{
			if(!ModelState.IsValid)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				return View(product);
			}

			var existProduct= await _context.Products.FindAsync(product.Id);
			if(existProduct is null)
				return BadRequest();

			var isExistCategory = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
			if (!isExistCategory)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
				return View(product);
			}
			existProduct.Name= product.Name;
			existProduct.Description= product.Description;
			existProduct.Price= product.Price;
			existProduct.ImagePath= product.ImagePath;
			existProduct.CategoryId= product.CategoryId;
			_context.Products.Update(existProduct);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
