namespace Pronia_example.Abstraction
{
	public interface IBasketService
	{
		Task<List<BasketItem>> GetBasketItemsAsync();
	}
}
