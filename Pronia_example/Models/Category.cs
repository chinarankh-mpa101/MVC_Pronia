using Pronia_example.Models.Common;
using System.ComponentModel.DataAnnotations;

public class Category : BaseEntity
{
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
}
