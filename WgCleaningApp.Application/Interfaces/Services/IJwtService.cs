namespace WgCleaningApp.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string role, string name);
}