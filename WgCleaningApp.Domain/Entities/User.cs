using WgCleaningApp.Domain.Enums;

namespace WgCleaningApp.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid WgId { get; private set; }

    public Wg? Wg { get; private set; }
    public string? DeviceToken { get; private set; }

    private User() { }

    public User(string name, string email, string passwordHash, UserRole role, Guid wgId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        WgId = wgId;
        DeviceToken = null; // new users have no token yet
    }

    public void UpdateDeviceToken(string token)
    {
        DeviceToken = token;
    }
    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
    }

}
