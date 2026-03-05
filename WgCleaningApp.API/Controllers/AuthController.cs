using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WgCleaningApp.Application.DTOs.Auth;
using WgCleaningApp.Application.Interfaces.Services;
using WgCleaningApp.Domain.Entities;
using WgCleaningApp.Domain.Enums;
using WgCleaningApp.Infrastructure.Persistence;

namespace WgCleaningApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthController(
        AppDbContext context,
        IPasswordHasher hasher,
        IJwtService jwt)
    {
        _context = context;
        _hasher = hasher;
        _jwt = jwt;
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var existingWg = await _context.Wgs
        .FirstOrDefaultAsync(w => w.Name.ToLower() == request.WgName.ToLower());

        Wg wg;
        UserRole role;

        if (existingWg == null)
        {
            // First person creating this WG → Admin
            wg = new Wg(request.WgName);
            role = UserRole.Admin;

            _context.Wgs.Add(wg);
        }
        else
        {
            // WG already exists → Normal User
            wg = existingWg;
            role = UserRole.Member;
        }
        var hashedPassword = _hasher.Hash(request.Password);

        var user = new User(
            request.Name,
            request.Email,
            hashedPassword,
            role,
            wg.Id
            
        );

     
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var token = _jwt.GenerateToken(
                     user.Id,
                     user.Email,
                     user.Role.ToString(),
                     user.Name
         );

        return Ok(new AuthResponse(
            token,
            user.Email, 
            user.Role.ToString()
         ));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
            return Unauthorized("Invalid credentials");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(
                    user.Id,
                    user.Email,
                    user.Role.ToString(),
                    user.Name
        );

        Console.WriteLine(user?.PasswordHash);
        Console.WriteLine($"Email: {request.Email}, Password: {request.Password}");


        return Ok(new AuthResponse(
             token,
             user.Email,
             user.Role.ToString()
        ));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Wg)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return BadRequest("User not found.");

        if (!string.Equals(user.Wg.Name, request.WgName, StringComparison.OrdinalIgnoreCase))
            return BadRequest("WG name does not match.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatePassword(newHash);

        await _context.SaveChangesAsync();

        return Ok("Password updated successfully.");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] AdminResetPasswordRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) return NotFound();

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatePassword(newHash);

        await _context.SaveChangesAsync();

        return Ok("Password reset successfully.");
    }

    public class AdminResetPasswordRequest
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }



}