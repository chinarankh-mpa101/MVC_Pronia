using Microsoft.EntityFrameworkCore;
using Pronia_example.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia_example.Models
{
	public class Product:BaseEntity
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }

		[Required]
		[Precision(10,2)]
		[Range(0,double.MaxValue)]
		public decimal Price { get; set; }

		public Category Category { get; set; }
		[Required]
		public int CategoryId { get; set; }

		public string MainImagePath { get; set; }


		public string HoverImagePath { get; set; }


		[Range(1,5)]
		public int Rating { get; set; }

		public ICollection<Productİmage> Productİmages { get; set; } = [];
        public ICollection<ProductTag> ProductTags { get; set; } = [];

        //public ICollection<Productİmage> Productİmages { get; set; }
    }
}
