using Taskboard.Core.Models;
using Taskboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Taskboard.Api.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            app.MapGet("/api/v2/tasks", async (AppDbContext db) =>
                await db.TaskItems.Include(t => t.AssignedUser).ToListAsync()
            ).WithTags("Minimal API - Tasks");

            app.MapPost("/api/v2/tasks", async (AppDbContext db, TaskItem task) =>
            {
                db.TaskItems.Add(task);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v2/tasks/{task.Id}", task);
            }).WithTags("Minimal API - Tasks");

            app.MapDelete("/api/v2/tasks/{id:int}", async (AppDbContext db, int id) =>
            {
                var task = await db.TaskItems.FindAsync(id);
                if (task == null) return Results.NotFound();
                db.TaskItems.Remove(task);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).WithTags("Minimal API - Tasks");
        }
    }
}