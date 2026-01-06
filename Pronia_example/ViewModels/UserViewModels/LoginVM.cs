using System.ComponentModel.DataAnnotations;

namespace Pronia_example.ViewModels.UserViewModels
{
    public class LoginVM
    {
        [Required, MaxLength(256), MinLength(3), EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required, MaxLength(32), MinLength(6), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; }
    }


}
