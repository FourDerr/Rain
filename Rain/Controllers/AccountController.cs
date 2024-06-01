using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rain.Models;
using Rain.Services;
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
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel newUser, string confirmPassword)
        {
            if (newUser.password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View();
            }

            _logger.LogInformation("Raw password: {Password}", newUser.password);
            newUser.password = BCrypt.Net.BCrypt.HashPassword(newUser.password);
            _logger.LogInformation("Hashed password: {HashedPassword}", newUser.password);

            await _userService.CreateUser(newUser);
            _logger.LogInformation("User created with email: {Email}", newUser.email);
            return RedirectToAction("SignIn");
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
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
        new Claim(ClaimTypes.Name, user.name), // Use the user's name here
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("User signed in successfully with email: {Email}", email);
            return RedirectToAction("Index", "Home");
        }




        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
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
