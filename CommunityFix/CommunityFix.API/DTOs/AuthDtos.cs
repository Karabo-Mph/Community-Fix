namespace CommunityFix.API.DTOs;

public record RegisterDto(string FullName, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponse(string Token, string Email, string FullName, IList<string> Roles);