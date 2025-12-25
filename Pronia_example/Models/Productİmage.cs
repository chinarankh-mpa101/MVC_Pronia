using Pronia_example.Models.Common;

namespace Pronia_example.Models
{
	public class Productİmage : BaseEntity
	{
		public int ProductId { get; set; }
		public Product product { get; set; }

		public string ImagePath { get; set; }

	}
}
