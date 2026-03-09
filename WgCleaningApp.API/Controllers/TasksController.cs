using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WgCleaningApp.Application.DTOs.Tasks;
//using WgCleaningApp.Application.DTOs.Assignment;
using WgCleaningApp.Domain.Entities;
using WgCleaningApp.Domain.Enums;
using WgCleaningApp.Infrastructure.Persistence;
using WgCleaningApp.Infrastructure.Services;


namespace WgCleaningApp.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public TasksController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTask(CreateTaskRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return Unauthorized();


            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user is null)
                return Unauthorized();

            if (user.Role != UserRole.Admin)
            {
                return StatusCode(403, new
                {
                    message = "Only Admin can create and assign tasks."
                });
            }

            var task = new TaskItem(
                request.Title,
                request.Description,
                user.WgId,
                request.StartDate,
                request.EndDate,
                request.AssignedToUserId
                );
            if (task.AssignedToUserId != null)
            {
                await _notificationService.SendNotificationAsync(
                    task.AssignedToUserId.Value,
                    $"A new task was assigned to you: {task.Title}"
                );
            }


            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Task deleted");
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user is null)
                return Unauthorized();

            var tasks = await _context.Tasks
                .Where(t => t.WgId == user.WgId)
                .Include(t => t.AssignedToUser)
                .ToListAsync();

            return Ok(tasks);
        }


        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var guid = Guid.Parse(userId);

            var tasks = await _context.Tasks
                .Where(t => t.AssignedToUserId == guid)
                .ToListAsync();

            return Ok(tasks);
        }


        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null)
                return NotFound();

            task.MarkCompleted();
            await _context.SaveChangesAsync();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            await _notificationService.SendNotificationToWgAsync(
                task.WgId,
                $"{user.Name} has just done cleaning: {task.Title}"
            );

            return Ok(task);
        }



        [HttpPut("{taskId}/assign/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            task.AssignTo(userId);
            await _context.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                userId,
                $"You have been assigned: {task.Title}"
            );

            return Ok(task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto dto)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return NotFound();

            task.UpdateDetails(dto.Title, dto.Description, dto.StartDate, dto.EndDate);

            if (dto.AssignedToUserId.HasValue)
                task.UpdateAssignedUser(dto.AssignedToUserId);

            await _context.SaveChangesAsync();

            return Ok(task);
        }




    }
}