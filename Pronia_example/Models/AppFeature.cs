using System.ComponentModel.DataAnnotations;

namespace Pronia_example.Models
{
    public class AppFeature
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [MinLength(5,ErrorMessage ="Minumum uzunluqu 5 olmalidirr")]
        public string Title { get; set; } = null;
        public string? Description { get; set; } = null;

        [Required(ErrorMessage ="Image bosh ola bilmez")]
        public string ImageUrl { get; set; } = null;

    }
}

