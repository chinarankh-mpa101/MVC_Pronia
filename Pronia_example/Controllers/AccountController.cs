using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Pronia_example.Contexts;
using Pronia_example.ViewModels.UserViewModels;

namespace Pronia_example.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var existUser = await _userManager.FindByNameAsync(vm.UserName);

            if(existUser is not null)
            {
                ModelState.AddModelError("UserName", "This username is already exists");
                return View(vm);
            }

            existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if(existUser is not null)
            {
                ModelState.AddModelError(nameof(vm.EmailAddress),"This email is already exists");
                return View(vm);
            }
            AppUser newUser = new()
            {
                Fullname = vm.FirstName + " " + vm.LastName,
                Email = vm.EmailAddress,
                UserName= vm.UserName

            };
            var result = await _userManager.CreateAsync(newUser, vm.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }
            // signin async dediyime gore birbasa registerden home-a gedecek
            await _signInManager.SignInAsync(newUser, false);
            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)

                return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (user is null)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(vm);
            }

            var loginResult = await _userManager.CheckPasswordAsync(user, vm.Password);
            if (!loginResult)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(vm);
            }
            await _signInManager.SignInAsync(user, vm.IsRemember);
            return Ok($"{user.Fullname} welcome");

            
        }

        public async Task <IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));

        }

        //public async Task<IActionResult> CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole()
        //    {
        //        Name="User"

        //    });
        //    await _roleManager.CreateAsync(new IdentityRole()
        //    {
        //        Name="Admin"
        //    });
        //    await _roleManager.CreateAsync(new IdentityRole()
        //    {
        //        Name = "Moderator"
        //    });
        //    return Ok("Roles created");

        //}

        // Her sey ela isleyir adminlik verdim oz adimla yaratdigim username-e ammaki home sehifesinde admin sozu cixmiree layoutda da 
        // yazmisamki user. isinrole admindise asp are admin, asp controller dashboard asp-action index amma yenede home-da admin sozu gorunmur
        // ammaki admin/dashboard edende gedir adminin dashboard-na
    }
}
