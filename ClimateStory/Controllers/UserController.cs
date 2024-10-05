
using Microsoft.AspNetCore.Mvc;
using ClimateStory.Models;
using ClimateStory.Services;
using Microsoft.EntityFrameworkCore;

namespace ClimateStory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        // Inject the UserService via dependency injection
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppDbContext.User>>> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppDbContext.User>> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<AppDbContext.User>> PostUser(AppDbContext.User user)
        {
            await _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, AppDbContext.User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            await _userService.UpdateUser(user);

            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
        
        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<AppDbContext.User>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(user);
        }
    }
}