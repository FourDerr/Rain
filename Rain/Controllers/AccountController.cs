using Microsoft.AspNetCore.Mvc;
using Rain.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Rain.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _userService.IsEmailRegistered(model.email))
            {
                ModelState.AddModelError("email", "Email is already registered");
                return View(model);
            }

            var newUser = new UserModel
            {
                name = model.name,
                email = model.email,
                password = BCrypt.Net.BCrypt.HashPassword(model.password)
            };

            await _userService.CreateUser(newUser);
            _logger.LogInformation("User created with email: {Email}", newUser.email);
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _logger.LogInformation("SignIn method called with email: {Email}", email);
            var user = await _userService.Authenticate(email, password);
            if (user == null)
            {
                _logger.LogWarning("Invalid email or password for email: {Email}", email);
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("User signed in successfully with email: {Email}", email);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
