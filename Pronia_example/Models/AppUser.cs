using Microsoft.AspNetCore.Identity;

namespace Pronia_example.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
    }
}
