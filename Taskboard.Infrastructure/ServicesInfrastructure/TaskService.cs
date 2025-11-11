using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Taskboard.Core.Models;
using Taskboard.Core.Services;
using Taskboard.Infrastructure.Data;
namespace Taskboard.Infrastructure.ServicesInfrastructure
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(AppDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all tasks...");
            return await _context.TaskItems.Include(t => t.AssignedUser).ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.TaskItems.Include(t => t.AssignedUser)
                                           .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Task '{task.Title}' created successfully!");
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            _logger.LogWarning($"Task '{task.Title}' deleted.");
            return true;
        }
    }
}
