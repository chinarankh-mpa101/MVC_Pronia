using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pronia_example.ViewModels.ProductViewModels
{
    public class ProductUpdateVM()
	{
		public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; }

        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }

        public string? MainImagePath { get; set; }
        public string? HoverImagePath { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
