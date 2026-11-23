using CommunityFix.API.DTOs;
using CommunityFix.API.Models;
using CommunityFix.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityFix.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly TokenService _tokens;

    public AuthController(UserManager<ApplicationUser> users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Everyone self-registers as a Resident. Higher roles are assigned by an administrator.
        await _users.AddToRoleAsync(user, Roles.Resident);

        return await IssueToken(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);

        if (user is null || !await _users.CheckPasswordAsync(user, dto.Password))
            return Unauthorized(new { message = "Invalid email or password." });

        return await IssueToken(user);
    }

    private async Task<IActionResult> IssueToken(ApplicationUser user)
    {
        var roles = await _users.GetRolesAsync(user);
        var token = _tokens.CreateToken(user, roles);

        return Ok(new AuthResponse(token, user.Email!, user.FullName, roles));
    }
}