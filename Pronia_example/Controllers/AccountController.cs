using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Pronia_example.Abstraction;
using Pronia_example.Contexts;
using Pronia_example.ViewModels.UserViewModels;

namespace Pronia_example.Controllers
{
	public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IEmailService _emailService) : Controller
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

			if (existUser is not null)
			{
				ModelState.AddModelError("UserName", "This username is already exists");
				return View(vm);
			}

			existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);
			if (existUser is not null)
			{
				ModelState.AddModelError(nameof(vm.EmailAddress), "This email is already exists");
				return View(vm);
			}
			AppUser newUser = new()
			{
				Fullname = vm.FirstName + " " + vm.LastName,
				Email = vm.EmailAddress,
				UserName = vm.UserName

			};
			var result = await _userManager.CreateAsync(newUser, vm.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(vm);
			}

			// signin async dediyime gore birbasa registerden home-a gedecek

			await SendConfirmationMailAsync(newUser);
			TempData["SuccessMessage"] = "Registerden ugurla kecdiniz zehmet olmasa emailinizi tesdiqleyin";
			//ModelState.AddModelError("", "Please confirm your email");
			return RedirectToAction("Login");
		}

		public IActionResult Login()
		{
			
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM vm)
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
			if (!user.EmailConfirmed)
			{
				ModelState.AddModelError("", "Please confirm your email");
				await SendConfirmationMailAsync(user);
				return View(vm);
			}

			await _signInManager.SignInAsync(user, vm.IsRemember);
			return Ok($"{user.Fullname} welcome");


		}

		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));

		}
		private async Task SendConfirmationMailAsync(AppUser user)
		{
			string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			//string url = $"https://localhost:44382/Account/ConfirmEmail?token={token}&userId={user.Id}";
			var url = Url.Action("ConfirmEmail", "Account", new { token, userId = user.Id }, Request.Scheme);

			string emailBody = $@"

			 <!DOCTYPE html>
<html lang=""en"" style=""margin:0; padding:0;"">
<head>
    <meta charset=""UTF-8"" />
    <title>Email Confirmation</title>
</head>
<body style=""margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;"">

    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:white; border-radius:8px; overflow:hidden;"">

                    <!-- Header -->
                    <tr>
                        <td style=""background:#4CAF50; padding:20px; text-align:center; color:white; font-size:24px; font-weight:bold;"">
                            Email Confirmation
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style=""padding:30px; color:#333; font-size:16px; line-height:1.5;"">
                            <p>Hello,</p>
                            <p>Thank you for registering! Please click the button below to confirm your email address.</p>

                            <p style=""text-align:center; margin:30px 0;"">
                                <a href=""{url}"" 
                                   style=""display:inline-block; background:#4CAF50; color:white; padding:12px 25px; font-size:16px; 
                                          text-decoration:none; border-radius:5px;"">
                                    Confirm Email
                                </a>
                            </p>

                            <p>If the button above does not work, copy and paste this link into your browser:</p>
                            <p style=""word-break:break-all;"">
                                {url}
                            </p>

                            <p>Best regards,<br> Your Company</p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background:#f1f1f1; padding:15px; text-align:center; font-size:12px; color:#777;"">
                            © 2026 Your Company. All rights reserved.
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>

</body>
</html>

";
			await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

		}

		public async Task<IActionResult> ConfirmEmail(string token, string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return NotFound();
			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded)
			{
				//await SendConfirmationMailAsync(user);
				return BadRequest();
			}
			await _signInManager.SignInAsync(user, false);
			return RedirectToAction("Index", "Home");
			//return Ok($"token: {token}, \n userId{userId} ");
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
		// ammaki admin/dashboard edende gedir adminin dashboard-na ignore edin bunu artiq hell eledik




	}
}
