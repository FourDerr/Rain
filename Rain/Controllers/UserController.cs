using Microsoft.AspNetCore.Mvc;
using Rain.Models;

namespace Rain.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel newUser)
        {
            await _userService.CreateUser(newUser);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserModel updatedUser)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            updatedUser.Id = user.Id; // Ensure the ID remains the same
            await _userService.UpdateUser(id, updatedUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            await _userService.RemoveUser(id);
            return NoContent();
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserModel login)
        {
            var user = await _userService.Authenticate(login.email, login.password);
            if (user == null)
                return Unauthorized();
            return Ok(user);
        }
    }
}
