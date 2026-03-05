using System.Security.Cryptography.X509Certificates;

namespace WgCleaningApp.Application.DTOs.Auth;

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string WgName
);