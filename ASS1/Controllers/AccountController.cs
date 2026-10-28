using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ASS1.Models;
using ASS1.Services;

namespace ASS1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await _authService.Authenticate(model.Email, model.Password);

                if (account != null)
                {
                    var role = _authService.GetUserRole((int)account.AccountRole);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.AccountName),
                        new Claim(ClaimTypes.Email, account.AccountEmail),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("AccountID", account.AccountId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return role switch
                    {
                        "Admin" => RedirectToAction("Index", "Admin"),
                        "Staff" => RedirectToAction("Index", "Staff"),
                        "Lecturer" => RedirectToAction("Index", "Lecturer"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}