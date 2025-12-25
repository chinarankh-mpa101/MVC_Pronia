using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Pronia_example.Contexts;
using Pronia_example.Migrations;
using Pronia_example.ViewModels.ProductViewModels;
using System.Reflection;


namespace Pronia_example.Areas.Admin.Controllers
{
	[Area("Admin")]

	public class ProductController(AppDbContext _context, IWebHostEnvironment _enviroment) : Controller
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
		public async Task<IActionResult> Create(ProductCreateVM vm)
		{
			

			if (!ModelState.IsValid)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				return View(vm);
			}
			var isExistCategory= await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId);
			if (!isExistCategory)
			{
				var categories = await _context.Categories.ToListAsync();
				ViewBag.Categories = categories;
				ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
				return View(vm);
			}

			if (!vm.MainImage.ContentType.Contains("image"))
			{
				ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edin");
				return View(vm);
			}

			if(vm.MainImage.Length > 2 * 1024 * 1024)
			{
				ModelState.AddModelError("MainImage", "Sekil olcusu maksimum 2MB ola biler");
			}


			if (!vm.HoverImage.ContentType.Contains("image"))
			{
				ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edin");
				return View(vm);
			}

			if (vm.HoverImage.Length > 2 * 1024 * 1024)
			{
				ModelState.AddModelError("HoverImage", "Sekil olcusu maksimum 2MB ola biler");
			}

			string uniqueMainImageName = Guid.NewGuid().ToString() + vm.MainImage.FileName;
			string mainImagePath= @$"{_enviroment.WebRootPath}/assets/images/website-images/{uniqueMainImageName}";

			using FileStream mainStream = new FileStream(mainImagePath, FileMode.Create);
			await vm.MainImage.CopyToAsync(mainStream);


			


			string uniqueHoverImageName = Guid.NewGuid().ToString() + vm.HoverImage.FileName;
			string hoverImagePath = @$"{_enviroment.WebRootPath}/assets/images/website-images/{uniqueHoverImageName}";

			using FileStream hoverStream = new FileStream(hoverImagePath, FileMode.Create);
			await vm.HoverImage.CopyToAsync(hoverStream);


			Product product = new()
			{
				Name = vm.Name,
				Description = vm.Description,
				Price = vm.Price,
				CategoryId = vm.CategoryId,
				MainImagePath = uniqueMainImageName,
				HoverImagePath = uniqueHoverImageName,
				Rating = vm.Rating,

			};


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


			string folderPath= Path.Combine(_enviroment.WebRootPath,"assets", "images", "website-images");
			string mainImagePath= Path.Combine(folderPath, product.MainImagePath);
			string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);
			if (System.IO.File.Exists(mainImagePath))
			{
				System.IO.File.Delete(mainImagePath);
			}

			if (System.IO.File.Exists(hoverImagePath))
			{
				System.IO.File.Delete(hoverImagePath);
			}


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
			//existProduct.ImagePath= product.ImagePath;
			existProduct.CategoryId= product.CategoryId;
			_context.Products.Update(existProduct);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
