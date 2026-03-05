using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WgCleaningApp.Domain.Entities;
using WgCleaningApp.Infrastructure.Persistence;

/*namespace WgCleaningApp.API.Controllers;

[ApiController]
[Route("api/cleaning-tasks")]
[Authorize]
public class CleaningTasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public CleaningTasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(string name)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _context.Users.FindAsync(Guid.Parse(userId!));

        if (user == null)
            return Unauthorized();

        var task = new CleaningTask(name, user.WgId);

        _context.CleaningTasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FindAsync(Guid.Parse(userId!));

        var tasks = await _context.CleaningTasks
            .Where(t => t.WgId == user!.WgId)
            .ToListAsync();

        return Ok(tasks); 
    }
}*/