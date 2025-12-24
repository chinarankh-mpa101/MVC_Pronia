using Microsoft.EntityFrameworkCore;
using Pronia_example.Models.Common;
using System.ComponentModel.DataAnnotations;

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

		[Required]
		[MaxLength(512)]
		public string ImagePath { get; set; }

		public Category? Category { get; set; }
		[Required]
		public int CategoryId { get; set; }	
	}
}
