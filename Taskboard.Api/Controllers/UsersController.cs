using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskboard.Infrastructure.Data;
using Taskboard.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Taskboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Get all users with their tasks.</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all users")]
        [SwaggerResponse(200, "Users retrieved")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Tasks).ToListAsync();
        }

        /// <summary>Get a user by ID with tasks.</summary>
        /// <param name="id">User ID</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user by ID")]
        [SwaggerResponse(200, "User found")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return user;
        }

        /// <summary>Create a new user.</summary>
        /// <param name="user">User object</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user")]
        [SwaggerResponse(201, "User created")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>Update an existing user.</summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user object</param>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a user")]
        [SwaggerResponse(204, "User updated")]
        [SwaggerResponse(400, "ID mismatch")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Delete a user by ID.</summary>
        /// <param name="id">User ID</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a user")]
        [SwaggerResponse(204, "User deleted")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
