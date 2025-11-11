using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Taskboard.Api.Dtos;
using Taskboard.Core.Models;
using Taskboard.Core.Services;

namespace Taskboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>Get all tasks with their assigned users.</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all tasks", Description = "Returns all tasks with assigned users.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var tasks = await _taskService.GetAllAsync();
            return Ok(tasks);
        }

        /// <summary>Get a specific task by ID.</summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get task by ID", Description = "Returns a single task with assigned user.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        /// <summary>Create a new task.</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create new task", Description = "Creates a new task and saves it to the database.")]
        [SwaggerResponse(201, "Task created successfully")]
        [SwaggerRequestExample(typeof(CreateTaskDto), typeof(CreateTaskDtoExample))]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto dto)
        {
            var newTask = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate
            };

            var created = await _taskService.CreateAsync(newTask);
            _logger.LogInformation("Task '{Title}' created successfully.", created.Title);

            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }

        /// <summary>Delete a task by ID.</summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a task")]
        [SwaggerResponse(204, "Task deleted")]
        [SwaggerResponse(404, "Task not found")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            _logger.LogWarning("Task with ID {Id} deleted.", id);
            return NoContent();
        }
    }
}