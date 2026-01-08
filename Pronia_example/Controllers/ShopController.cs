using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia_example.Abstraction;
using Pronia_example.Contexts;
using Pronia_example.Models;
using Pronia_example.ViewModels.ProductViewModels;
using System.Security.Claims;

namespace Pronia_example.Controllers
{
	public class ShopController(AppDbContext _context, IEmailService _emailService) : Controller
	{
		public async Task<IActionResult> Index()
		{
			var products = await _context.Products.ToListAsync();
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
			if (product is null)
				return NotFound();
			return View(product);
		}

		public async Task<IActionResult> AddToBasket(int productId)
		{
			var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

			if (isExistProduct == false)
				return NotFound();
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

			var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
			if (!isExistUser)
			{
				return BadRequest();
			}

			var existBasketItems = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

			if (existBasketItems is not null)
			{
				existBasketItems.Count++;
				_context.BasketItems.Update(existBasketItems);
				await _context.SaveChangesAsync();
			}
			else
			{
				BasketItem basketItem = new()
				{
					ProductId = productId,
					AppUserId = userId,
					Count = 1

				};
				await _context.BasketItems.AddAsync(basketItem);
			}

			await _context.SaveChangesAsync();
            string? returnUrl = Request.Headers["Referer"];

			if (!string.IsNullOrWhiteSpace(returnUrl))
				return Redirect(returnUrl);
            return RedirectToAction("Index");
           
		}

		[Authorize]
		public async Task<IActionResult> RemoveFromBasket(int productId)
		{

			var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

			if (isExistProduct == false)
				return NotFound();
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

			var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
			if (!isExistUser)
			{
				return BadRequest();
			}
			var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId==productId );

			if(basketItem is null)
			
				return NotFound();
			_context.BasketItems.Remove(basketItem);
			await _context.SaveChangesAsync();

			string? returnUrl = Request.Headers["Referer"];

			if (!string.IsNullOrWhiteSpace(returnUrl))
				return Redirect(returnUrl);

			return RedirectToAction("Index");
		}
	}
}
