using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskboard.Infrastructure.Data;
using Taskboard.Core.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Taskboard.Api.Dtos;

namespace Taskboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Get all tasks with their assigned users.</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all tasks", Description = "Returns all tasks with assigned users.")]
        [SwaggerResponse(200, "Tasks successfully retrieved")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            return await _context.TaskItems.Include(t => t.AssignedUser).ToListAsync();
        }

        /// <summary>Get a specific task by ID.</summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get task by ID", Description = "Returns a single task with assigned user.")]
        [SwaggerResponse(200, "Task found")]
        [SwaggerResponse(404, "Task not found")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _context.TaskItems
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            return task;
        }

        /// <summary>Create a new task.</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create new task", Description = "Creates a new task and saves it to the database.")]
        [SwaggerResponse(201, "Task created successfully")]
        [SwaggerRequestExample(typeof(CreateTaskDto), typeof(CreateTaskDtoExample))]
        public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>Update an existing task.</summary>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a task")]
        [SwaggerResponse(204, "Task updated")]
        [SwaggerResponse(400, "ID mismatch")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            if (id != task.Id)
                return BadRequest();

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>Delete a task by ID.</summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a task")]
        [SwaggerResponse(204, "Task deleted")]
        [SwaggerResponse(404, "Task not found")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
                return NotFound();

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
