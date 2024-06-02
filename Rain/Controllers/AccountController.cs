using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rain.Models;
using Rain.Services;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Rain.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<AccountController> _logger;
        private readonly SmtpSettings _smtpSettings;

        public AccountController(UserService userService, ILogger<AccountController> logger, IOptions<SmtpSettings> smtpSettings)
        {
            _userService = userService;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
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

            var emailConfirmationToken = Guid.NewGuid().ToString();

            var newUser = new UserModel
            {
                name = model.name,
                email = model.email,
                password = BCrypt.Net.BCrypt.HashPassword(model.password),
                isEmailConfirmed = false,
                emailConfirmationToken = emailConfirmationToken
            };

            await _userService.CreateUser(newUser);
            _logger.LogInformation("User created with email: {Email}", newUser.email);

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, token = emailConfirmationToken }, protocol: HttpContext.Request.Scheme);
            await SendEmailConfirmationAsync(model.email, callbackUrl);

            ViewBag.Message = "Registration successful. Please check your email to confirm your email address.";
            return View("Info");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null || user.emailConfirmationToken != token)
            {
                ViewBag.ErrorMessage = "Invalid email confirmation request.";
                return View("Error");
            }

            user.isEmailConfirmed = true;
            user.emailConfirmationToken = null;
            await _userService.UpdateUser(user.Id, user);

            ViewBag.Message = "Email confirmed successfully. You can now log in.";
            return View("Info");
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
            if (user == null || !user.isEmailConfirmed)
            {
                _logger.LogWarning("Invalid email or password or email not confirmed for email: {Email}", email);
                ModelState.AddModelError("", "Invalid email or password or email not confirmed.");
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

        private async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName);
            message.Subject = "Confirm your email";
            message.Body = $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>.";
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                smtpClient.EnableSsl = _smtpSettings.EnableSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
