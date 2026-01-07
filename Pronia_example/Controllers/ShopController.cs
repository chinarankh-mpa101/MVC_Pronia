using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_example.Abstraction;
using Pronia_example.Contexts;
using Pronia_example.ViewModels.ProductViewModels;

namespace Pronia_example.Controllers
{
	public class ShopController(AppDbContext _context, IEmailService _emailService ) : Controller
	{
		public async Task <IActionResult> Index()
		{
			var products= await _context.Products.ToListAsync();
			return View(products);
		}

		public async Task<IActionResult> Test()
		{
			await _emailService.SendEmailAsync("chinarankh-mpa101@code.edu.az", "MPA-101", "<h1 style='color:red'> Email service is done</h1>");
			return Ok("Ok");
		}


		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var product = await _context.Products.Select(x => new ProductGetVM()
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
				AdditionalImagePaths = x.Productİmages.Select(x => x.ImagePath).ToList(),
				CategoryName = x.Category.Name,
				HoverImagePath = x.HoverImagePath,
				MainImagePath = x.MainImagePath,
				Price = x.Price,
				TagNames = x.ProductTags.Select(x => x.Tag.Name).ToList()

			}).FirstOrDefaultAsync(x => x.Id == id);
			if(product is null)
				return NotFound();
			return View(product);
		}
	}
}
