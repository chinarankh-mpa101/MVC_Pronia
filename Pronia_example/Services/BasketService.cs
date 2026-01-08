using Microsoft.EntityFrameworkCore;
using Pronia_example.Abstraction;
using Pronia_example.Contexts;
using System.Security.Claims;

namespace Pronia_example.Services
{
	public class BasketService(AppDbContext _context, IHttpContextAccessor _accessor):IBasketService
	{
		public async Task<List<BasketItem>> GetBasketItemsAsync()
		{
			string userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";


			var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

			if (!isExistUser)
				return [];

			var basketItems = await _context.BasketItems.Include(x=>x.Product).Where(x => x.AppUserId == userId).ToListAsync();

			return basketItems;
		}
	}
}
