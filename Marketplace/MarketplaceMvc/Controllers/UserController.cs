using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MarketplaceMvc.Security;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly MarketplaceContext _context;

        public UserController(MarketplaceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["HideNavbar"] = true;
            var userLoginVM = new UserLoginVM { ReturnUrl = returnUrl };
            return View(userLoginVM);
        }

        [HttpPost]
        public IActionResult Login(UserLoginVM userLoginVM)
        {
            var existingUser = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Username == userLoginVM.Username);
            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var b64hash = PasswordHashProvider.GetHash(userLoginVM.Password, existingUser.PwdSalt);
            if (b64hash != existingUser.PwdHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, userLoginVM.Username),
                new Claim(ClaimTypes.Role, existingUser.Role.RoleName),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
                );

            var authProperties = new AuthenticationProperties();

            Task.Run(async () =>
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties)
            ).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(userLoginVM.ReturnUrl))
            {
                return LocalRedirect(userLoginVM.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }

        public IActionResult Register()
        {
            ViewData["HideNavbar"] = true;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Register(UserVM userVm)
        {
            var trimmedUsername = userVm.Username.Trim();

            if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
            {
                ModelState.AddModelError("", "This username already exists");
                return View();
            }

            return RedirectToAction("ConfirmRegistration", userVm);
        }

        public ActionResult ConfirmRegistration(UserVM userVm)
        {
            return View(userVm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CompleteRegistration(UserVM userVm)
        {
            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash(userVm.Password, b64salt);

            var user = new User
            {
                Username = userVm.Username,
                PwdHash = b64hash,
                PwdSalt = b64salt,
                FirstName = userVm.FirstName,
                LastName = userVm.LastName,
                Email = userVm.Email,
                Phone = userVm.Phone,
                RoleId = 1
            };

            _context.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";

            return RedirectToAction("Login");
        }
    }
}
